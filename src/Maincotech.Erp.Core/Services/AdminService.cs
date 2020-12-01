using Maincotech.Caching;
using Maincotech.Cms;
using Maincotech.Cms.Dto;
using Maincotech.Cms.Models;
using Maincotech.Data;
using Maincotech.Domain.Repositories;
using Maincotech.Domain.Specifications;
using Maincotech.EF;
using Maincotech.Erp.Dto;
using Maincotech.Erp.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Maincotech.Erp.Services
{
    public class AdminService : IAdminService
    {
        private readonly EntityFrameworkRepositoryContext _repositoryContext;
        private ICacheManager _cacheManager;
        private readonly IRepository<Product> _productRepository;
        private readonly IRepository<Tag> _tagRepository;
        private readonly IRepository<ProductTag> _productTagRepository;
        private readonly IRepository<ProductImage> _productImageRepository;
        private readonly IRepository<Attachment> _attachmentRepository;
        private readonly IRepository<Category> _categoryRepository;

        public AdminService(DbContext erpDbContext, ICacheManager cacheManager = null)
        {
            _cacheManager = cacheManager;
            _repositoryContext = new EntityFrameworkRepositoryContext(erpDbContext);
            _productRepository = new EntityFrameworkRepository<Product>(_repositoryContext);
            _productTagRepository = new EntityFrameworkRepository<ProductTag>(_repositoryContext);
            _productImageRepository = new EntityFrameworkRepository<ProductImage>(_repositoryContext);
            _tagRepository = new EntityFrameworkRepository<Tag>(_repositoryContext);
            _attachmentRepository = new EntityFrameworkRepository<Attachment>(_repositoryContext);
            _categoryRepository = new EntityFrameworkRepository<Category>(_repositoryContext);
        }

        public async Task CreateOrUpdateProduct(ProductDto dto)
        {
            var entity = dto.To<Product>();
            _productRepository.AddOrUpdate(entity);

            var tagEntities = new List<Tag>();
            if (dto.Tags.IsNotNullOrEmpty())
            {
                var tags = dto.Tags.Split(new char[] { ',' });
                foreach (var tag in tags)
                {
                    var tagEntity = _tagRepository.Find(CmsSpecifications.TagsWithNameAndType(tag, TagType.Public));
                    if (tagEntity == null)
                    {
                        tagEntity = new Tag() { Id = Guid.NewGuid(), TagType = TagType.Public, Name = tag };
                        _tagRepository.Add(tagEntity);
                        tagEntities.Add(tagEntity);
                    }
                }
            }

            var associatedTags = _productTagRepository.FindAll(ErpSpecifications.TagsWithProductId(dto.Id)).ToList();
            var tagsToBeDeleted = associatedTags.Where(x => tagEntities.Any(tag => tag.Id == x.TargetId) == false).Select(x => x.Id).ToList();
            var tagsNeedToBeAdded = tagEntities.Where(x => associatedTags.Any(a => a.TargetId == x.Id) == false).ToList();

            if (tagsToBeDeleted.Any())
            {
                _productTagRepository.RemoveAll(DomainObjectSpecifications.IdIn<ProductTag>(tagsToBeDeleted));
            }
            if (tagsNeedToBeAdded.Any())
            {
                foreach (var tagId in tagsNeedToBeAdded)
                {
                    var association = new ProductTag
                    {
                        Id = Guid.NewGuid(),
                        SourceId = dto.Id,
                        TargetId = tagId.Id
                    };
                    _productTagRepository.Add(association);
                }
            }

            //Product Images
            var images = dto.Images?.Select(x => Guid.Parse(x.Id)).ToList() ?? new List<Guid>();
            var associatedImages = _productImageRepository.FindAll(ErpSpecifications.ImageWithProductId(dto.Id)).ToList();
            var imagesToBeDeleted = associatedImages.Where(x => images.Any(image => image == x.TargetId) == false).Select(x => x.Id).ToList();
            var imagesToBeAdded = images.Where(x => associatedImages.Any(a => a.TargetId == x) == false).ToList();
            if (imagesToBeDeleted.Any())
            {
                _productImageRepository.RemoveAll(DomainObjectSpecifications.IdIn<ProductImage>(imagesToBeDeleted));
            }
            if (imagesToBeAdded.Any())
            {
                foreach (var item in imagesToBeAdded)
                {
                    var image = dto.Images.First(x => x.Id == item.ToString());
                    var attachement = new Attachment
                    {
                        Id = item,
                        Name = image.Name,
                        RbsInfo = image.Url
                    };
                    _attachmentRepository.AddOrUpdate(attachement);

                    var association = new ProductImage
                    {
                        Id = Guid.NewGuid(),
                        SourceId = dto.Id,
                        TargetId = item
                    };
                    _productImageRepository.Add(association);
                }
            }
            _repositoryContext.Commit();
        }

        public async Task<ProductDto> GetProduct(Guid id)
        {
            var entity = _productRepository.GetByKey(id);
            if (entity == null)
            {
                return (default);
            }
            var dto = entity.To<ProductDto>();
            var tags = _productTagRepository.FindAll(ErpSpecifications.TagsWithProductId(id), x => x.Target).Select(x => x.Target.Name);
            if (tags.Any())
            {
                dto.Tags = string.Join(",", tags);
            }
            dto.Images = _productImageRepository.FindAll(ErpSpecifications.ImageWithProductId(id), x => x.Target).Select(x => new ProductImageDto
            {
                Id = x.TargetId.ToString(),
                Name = x.Target.Name,
                Url = x.Target.RbsInfo
            }).ToList();
            return dto;
        }

        public async Task<PagedResult<ProductDto>> GetProducts(PagingQuery pagingQuery)
        {
            var entities = _productRepository.GetPagedResult(pagingQuery);
            var dtos = entities.To<PagedResult<ProductDto>>();
            foreach(var dto in dtos)
            {
                var tags = _productTagRepository.FindAll(ErpSpecifications.TagsWithProductId(dto.Id), x => x.Target).Select(x => x.Target.Name);
                if (tags.Any())
                {
                    dto.Tags = string.Join(",", tags);
                }
                dto.Images = _productImageRepository.FindAll(ErpSpecifications.ImageWithProductId(dto.Id), x => x.Target).Select(x => new ProductImageDto
                {
                    Id = x.TargetId.ToString(),
                    Name = x.Target.Name,
                    Url = x.Target.RbsInfo
                }).ToList();
            }
            return dtos;
        }

        public async Task AddProductImage(ProductImageDto dto)
        {
            var entity = dto.To<Attachment>();
            _attachmentRepository.AddOrUpdate(entity);
            _repositoryContext.Commit();
        }

        public async Task<IEnumerable<CategoryDto>> GetProductCategories()
        {
            var ids = Enumerable.Select(_repositoryContext.Context.Set<Product>(), x => x.CategoryId).Distinct().ToList();
            if (ids.Count == 0)
            {
                return Array.Empty<CategoryDto>();
            }
            var entities = _categoryRepository.FindAll(DomainObjectSpecifications.IdIn<Category>(ids)).ToList();
            return entities.To<List<CategoryDto>>();
        }
    }
}