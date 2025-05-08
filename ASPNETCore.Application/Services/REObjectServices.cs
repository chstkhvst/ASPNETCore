using ASPNETCore.Application.DTO;
using ASPNETCore.Domain.Entities;
using ASPNETCore.Domain.Interfaces;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using ASPNETCore.Domain;

namespace ASPNETCore.Application.Services
{
    /// <summary>
    /// Сервис для работы с объектами недвижимости
    /// </summary>
    public class REObjectServices
    {
        private readonly IREObjectRepository _reObjectRepository;
        private readonly IImageRepository _imageRepository;

        /// <summary>
        /// Инициализирует новый экземпляр <see cref="REObjectServices"/>
        /// </summary>
        /// <param name="reObjectRepository">Репозиторий объектов недвижимости</param>
        /// <param name="imageRepository">Репозиторий изображений</param>
        public REObjectServices(IREObjectRepository reObjectRepository, IImageRepository imageRepository)
        {
            _reObjectRepository = reObjectRepository;
            _imageRepository = imageRepository;
        }

        /// <summary>
        /// Получает все объекты недвижимости
        /// </summary>
        /// <param name="isAdmin">Флаг, указывающий на права администратора</param>
        /// <returns>Коллекция DTO объектов недвижимости</returns>
        /// <remarks>
        /// Для пользователей без роли администратора возвращаются только активные объекты (statusId = 1)
        /// </remarks>
        public async Task<IEnumerable<REObjectDTO>> GetAllAsync(bool isAdmin = false)
        {
            var objects = isAdmin
                ? await _reObjectRepository.GetAllAsync()
                : await _reObjectRepository.GetFilteredAsync(null, null, 1);

            return objects.Select(o => new REObjectDTO
            {
                Id = o.Id,
                Rooms = o.Rooms,
                Floors = o.Floors,
                Building = o.Building,
                Roomnum = o.Roomnum,
                Square = o.Square,
                Street = o.Street,
                DealTypeId = o.DealTypeId,
                Price = o.Price,
                TypeId = o.TypeId,
                StatusId = o.StatusId,
                Status = o.Status != null ? new Status { Id = o.Status.Id, StatusName = o.Status.StatusName } : null,
                ObjectType = o.ObjectType != null ? new ObjectType { Id = o.ObjectType.Id, TypeName = o.ObjectType.TypeName } : null,
                DealType = o.DealType != null ? new DealType { Id = o.DealType.Id, DealName = o.DealType.DealName } : null,
                ObjectImages = o.ObjectImages?.Select(i => new ObjectImagesDTO
                {
                    Id = i.Id,
                    ImagePath = i.ImagePath,
                    ObjectId = i.ObjectId
                }).ToList() ?? new List<ObjectImagesDTO>()
            });
        }

        /// <summary>
        /// Получает объект недвижимости по идентификатору
        /// </summary>
        /// <param name="id">Идентификатор объекта</param>
        /// <returns>DTO объекта недвижимости или null, если не найден</returns>
        public async Task<REObjectDTO> GetByIdAsync(int id)
        {
            var obj = await _reObjectRepository.GetByIdAsync(id);
            if (obj == null)
            {
                Console.WriteLine($"Объект с ID {id} не найден в БД!");
                return null;
            }

            return new REObjectDTO
            {
                Id = obj.Id,
                Rooms = obj.Rooms,
                Floors = obj.Floors,
                Building = obj.Building,
                Roomnum = obj.Roomnum,
                Square = obj.Square,
                Street = obj.Street,
                DealTypeId = obj.DealTypeId,
                Price = obj.Price,
                TypeId = obj.TypeId,
                StatusId = obj.StatusId,
                Status = obj.Status != null ? new Status { Id = obj.Status.Id, StatusName = obj.Status.StatusName } : null,
                ObjectType = obj.ObjectType != null ? new ObjectType { Id = obj.ObjectType.Id, TypeName = obj.ObjectType.TypeName } : null,
                DealType = obj.DealType != null ? new DealType { Id = obj.DealType.Id, DealName = obj.DealType.DealName } : null,
                ObjectImages = obj.ObjectImages?.Select(i => new ObjectImagesDTO
                {
                    Id = i.Id,
                    ImagePath = i.ImagePath,
                    ObjectId = i.ObjectId
                }).ToList() ?? new List<ObjectImagesDTO>()
            };
        }

        /// <summary>
        /// Ищет объекты недвижимости по названию улицы
        /// </summary>
        /// <param name="street">Название улицы для поиска</param>
        /// <returns>Коллекция DTO найденных объектов</returns>
        public async Task<IEnumerable<REObjectDTO>> SearchByNameAsync(string street)
        {
            var objects = await _reObjectRepository.SearchByNameAsync(street);
            return objects.Select(o => new REObjectDTO
            {
                Id = o.Id,
                Rooms = o.Rooms,
                Floors = o.Floors,
                Building = o.Building,
                Roomnum = o.Roomnum,
                Square = o.Square,
                Street = o.Street,
                DealTypeId = o.DealTypeId,
                Price = o.Price,
                TypeId = o.TypeId,
                StatusId = o.StatusId
            });
        }

        /// <summary>
        /// Сохраняет изображение на сервере
        /// </summary>
        /// <param name="file">Файл изображения</param>
        /// <param name="env">Окружение веб-хоста</param>
        /// <returns>Путь к сохраненному изображению</returns>
        private async Task<string> SaveImage(IFormFile file, IWebHostEnvironment env)
        {
            var uploadsFolder = Path.Combine(env.WebRootPath, "assets");
            if (!Directory.Exists(uploadsFolder))
                Directory.CreateDirectory(uploadsFolder);

            var uniqueFileName = Guid.NewGuid() + Path.GetExtension(file.FileName);
            var filePath = Path.Combine(uploadsFolder, uniqueFileName);

            using (var stream = new FileStream(filePath, FileMode.Create))
            {
                await file.CopyToAsync(stream);
            }

            return $"/assets/{uniqueFileName}";
        }

        /// <summary>
        /// Добавляет новый объект недвижимости
        /// </summary>
        /// <param name="o">DTO для создания объекта</param>
        /// <param name="files">Коллекция файлов изображений</param>
        /// <param name="env">Окружение веб-хоста</param>
        /// <returns>Асинхронная задача</returns>
        public async Task AddAsync(CreateREObjectDTO o, IFormFileCollection? files = null, IWebHostEnvironment? env = null)
        {
            var obj = new REObject
            {
                Id = o.Id,
                Rooms = o.Rooms,
                Floors = o.Floors,
                Building = o.Building,
                Roomnum = o.Roomnum,
                Square = o.Square,
                Street = o.Street,
                DealTypeId = o.DealTypeId,
                Price = o.Price,
                TypeId = o.TypeId,
                StatusId = o.StatusId,
                ObjectImages = o.ObjectImages?.Select(img => new ObjectImages
                {
                    ImagePath = img.ImagePath,
                    ObjectId = o.Id
                }).ToList() ?? new List<ObjectImages>()
            };

            await _reObjectRepository.AddAsync(obj);
            var savedId = obj.Id;

            if (files != null && env != null)
            {
                foreach (var file in files)
                {
                    var imagePath = await SaveImage(file, env);
                    await _imageRepository.AddAsync(new ObjectImages
                    {
                        ImagePath = imagePath,
                        ObjectId = savedId
                    });
                }
            }
        }

        /// <summary>
        /// Обновляет объект недвижимости
        /// </summary>
        /// <param name="o">DTO с обновленными данными</param>
        /// <param name="files">Коллекция новых файлов изображений</param>
        /// <param name="env">Окружение веб-хоста</param>
        /// <param name="imagesToDelete">Список идентификаторов изображений для удаления</param>
        /// <returns>Асинхронная задача</returns>
        public async Task UpdateAsync(
            CreateREObjectDTO o,
            IFormFileCollection? files = null,
            IWebHostEnvironment? env = null,
            IEnumerable<int>? imagesToDelete = null)
        {
            var obj = await _reObjectRepository.GetByIdAsync(o.Id);
            if (obj == null)
                return;

            obj.Rooms = o.Rooms;
            obj.Floors = o.Floors;
            obj.Building = o.Building;
            obj.Roomnum = o.Roomnum;
            obj.Square = o.Square;
            obj.Street = o.Street;
            obj.DealTypeId = o.DealTypeId;
            obj.Price = o.Price;
            obj.TypeId = o.TypeId;
            obj.StatusId = o.StatusId;

            if (imagesToDelete != null && imagesToDelete.Any())
            {
                foreach (int id in imagesToDelete)
                    await _imageRepository.DeleteAsync(id);
            }

            if (files != null && files.Count > 0 && env != null)
            {
                foreach (var file in files)
                {
                    var imagePath = await SaveImage(file, env);
                    await _imageRepository.AddAsync(new ObjectImages
                    {
                        ImagePath = imagePath,
                        ObjectId = o.Id
                    });
                }
            }

            await _reObjectRepository.UpdateAsync(obj);
        }

        /// <summary>
        /// Удаляет объект недвижимости
        /// </summary>
        /// <param name="id">Идентификатор объекта</param>
        /// <exception cref="ApplicationException">Выбрасывается при ошибке удаления</exception>
        /// <returns>Асинхронная задача</returns>
        public async Task DeleteAsync(int id)
        {
            try
            {
                await _reObjectRepository.DeleteAsync(id);
            }
            catch (InvalidOperationException ex)
            {
                throw new ApplicationException($"Ошибка при удалении объекта: {ex.Message}", ex);
            }
        }

        /// <summary>
        /// Получает отфильтрованные объекты недвижимости
        /// </summary>
        /// <param name="typeId">Идентификатор типа объекта (опционально)</param>
        /// <param name="dealTypeId">Идентификатор типа сделки (опционально)</param>
        /// <param name="statusId">Идентификатор статуса (опционально)</param>
        /// <param name="isAdmin">Флаг, указывающий на права администратора</param>
        /// <returns>Коллекция DTO отфильтрованных объектов</returns>
        /// <remarks>
        /// Для пользователей без роли администратора используется statusId = 1 по умолчанию
        /// </remarks>
        public async Task<IEnumerable<REObjectDTO>> GetFilteredAsync(
            int? typeId,
            int? dealTypeId,
            int? statusId,
            bool isAdmin = false)
        {
            var effectiveStatusId = isAdmin ? statusId : 1;

            var objects = await _reObjectRepository.GetFilteredAsync(
                typeId,
                dealTypeId,
                effectiveStatusId);

            return objects.Select(o => new REObjectDTO
            {
                Id = o.Id,
                Rooms = o.Rooms,
                Floors = o.Floors,
                Building = o.Building,
                Roomnum = o.Roomnum,
                Square = o.Square,
                Street = o.Street,
                DealTypeId = o.DealTypeId,
                Price = o.Price,
                TypeId = o.TypeId,
                StatusId = o.StatusId,
                Status = o.Status != null ? new Status { Id = o.Status.Id, StatusName = o.Status.StatusName } : null,
                ObjectType = o.ObjectType != null ? new ObjectType { Id = o.ObjectType.Id, TypeName = o.ObjectType.TypeName } : null,
                DealType = o.DealType != null ? new DealType { Id = o.DealType.Id, DealName = o.DealType.DealName } : null,
                ObjectImages = o.ObjectImages?.Select(i => new ObjectImagesDTO
                {
                    Id = i.Id,
                    ImagePath = i.ImagePath,
                    ObjectId = i.ObjectId
                }).ToList() ?? new List<ObjectImagesDTO>()
            });
        }


        /// <summary>
        /// Получает объекты недвижимости с пагинацией
        /// </summary>
        /// <param name="isAdmin">Флаг, указывающий на права администратора</param>
        /// <param name="page">Номер страницы</param>
        /// <param name="pageSize">Размер страницы</param>
        /// <returns>Пагинированный ответ с DTO объектов недвижимости</returns>
        public async Task<PaginatedResponse<REObjectDTO>> GetAllPaginatedAsync(bool isAdmin = false, int page = 1, int pageSize = 5)
        {
            // Получаем пагинированные данные из репозитория
            var paginatedResult = isAdmin
                ? await _reObjectRepository.GetAllPaginatedAsync(page, pageSize)
                : await _reObjectRepository.GetFilteredPaginatedAsync(null, null, 1, page, pageSize);

            // Преобразуем в DTO
            var dtos = paginatedResult.Items.Select(o => new REObjectDTO
            {
                Id = o.Id,
                Rooms = o.Rooms,
                Floors = o.Floors,
                Building = o.Building,
                Roomnum = o.Roomnum,
                Square = o.Square,
                Street = o.Street,
                DealTypeId = o.DealTypeId,
                Price = o.Price,
                TypeId = o.TypeId,
                StatusId = o.StatusId,
                Status = o.Status != null ? new Status { Id = o.Status.Id, StatusName = o.Status.StatusName } : null,
                ObjectType = o.ObjectType != null ? new ObjectType { Id = o.ObjectType.Id, TypeName = o.ObjectType.TypeName } : null,
                DealType = o.DealType != null ? new DealType { Id = o.DealType.Id, DealName = o.DealType.DealName } : null,
                ObjectImages = o.ObjectImages?.Select(i => new ObjectImagesDTO
                {
                    Id = i.Id,
                    ImagePath = i.ImagePath,
                    ObjectId = i.ObjectId
                }).ToList() ?? new List<ObjectImagesDTO>()
            }).ToList();

            return new PaginatedResponse<REObjectDTO>
            {
                Items = dtos,
                TotalCount = paginatedResult.TotalCount,
                CurrentPage = page,
                TotalPages = paginatedResult.TotalPages
            };
        }
        public async Task<PaginatedResponse<REObjectDTO>> GetFilteredPaginatedAsync(
            int? typeId,
            int? dealTypeId,
            int? statusId,
            bool isAdmin = false,
            int page = 1,
            int pageSize = 5)
        {
            var effectiveStatusId = isAdmin ? statusId : 1;

            var paginatedResult = await _reObjectRepository.GetFilteredPaginatedAsync(
                typeId,
                dealTypeId,
                effectiveStatusId,
                page,
                pageSize);

            var dtos = paginatedResult.Items.Select(o => new REObjectDTO
            {
                Id = o.Id,
                Rooms = o.Rooms,
                Floors = o.Floors,
                Building = o.Building,
                Roomnum = o.Roomnum,
                Square = o.Square,
                Street = o.Street,
                DealTypeId = o.DealTypeId,
                Price = o.Price,
                TypeId = o.TypeId,
                StatusId = o.StatusId,
                Status = o.Status != null ? new Status { Id = o.Status.Id, StatusName = o.Status.StatusName } : null,
                ObjectType = o.ObjectType != null ? new ObjectType { Id = o.ObjectType.Id, TypeName = o.ObjectType.TypeName } : null,
                DealType = o.DealType != null ? new DealType { Id = o.DealType.Id, DealName = o.DealType.DealName } : null,
                ObjectImages = o.ObjectImages?.Select(i => new ObjectImagesDTO
                {
                    Id = i.Id,
                    ImagePath = i.ImagePath,
                    ObjectId = i.ObjectId
                }).ToList() ?? new List<ObjectImagesDTO>()
            }).ToList();

            return new PaginatedResponse<REObjectDTO>
            {
                Items = dtos,
                TotalCount = paginatedResult.TotalCount,
                CurrentPage = page,
                TotalPages = paginatedResult.TotalPages
            };

        }
    }
}