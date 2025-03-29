using ASPNETCore.Application.DTO;
using ASPNETCore.Domain.Entities;
using ASPNETCore.Domain.Interfaces;

namespace ASPNETCore.Application.Services
{
    public class REObjectServices
    {
        private readonly IREObjectRepository _reObjectRepository;

        public REObjectServices(IREObjectRepository reObjectRepository)
        {
            _reObjectRepository = reObjectRepository;
        }

        //Получение всех объектов недвижимости
        public async Task<IEnumerable<REObjectDTO>> GetAllAsync()
        {
            var objects = await _reObjectRepository.GetAllAsync();
            return objects.Select(o => new REObjectDTO
            {
                Id = o.Id,
                Rooms = o.Rooms,
                Floors = o.Floors,
                Building = o.Building,
                Number = o.Number,
                Square = o.Square,
                Street = o.Street,
                DealTypeId = o.DealTypeId,
                Price = o.Price,
                TypeId = o.TypeId,
                StatusId = o.StatusId,
                Status = new Status { Id = o.Status.Id, StatusName = o.Status.StatusName },
                ObjectType = new ObjectType { Id = o.TypeId, TypeName = o.ObjectType.TypeName },
                DealType = new DealType { Id = o.TypeId, DealName = o.DealType.DealName }
            });
        }


        // Получение объекта по ID
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
                Number = obj.Number,
                Square = obj.Square,
                Street = obj.Street,
                DealTypeId = obj.DealTypeId,
                Price = obj.Price,
                TypeId = obj.TypeId,
                StatusId = obj.StatusId
            };
        }

        // Поиск объектов по названию
        public async Task<IEnumerable<REObjectDTO>> SearchByNameAsync(string street)
        {
            var objects = await _reObjectRepository.SearchByNameAsync(street);
            return objects.Select(o => new REObjectDTO
            {
                Id = o.Id,
                Rooms = o.Rooms,
                Floors = o.Floors,
                Building = o.Building,
                Number = o.Number,
                Square = o.Square,
                Street = o.Street,
                DealTypeId = o.DealTypeId,
                Price = o.Price,
                TypeId = o.TypeId,
                StatusId = o.StatusId
            });
        }

        // Добавление нового объекта
        public async Task AddAsync(CreateREObjectDTO o)
        {
            var obj = new REObject
            {
                Id = o.Id,
                Rooms = o.Rooms,
                Floors = o.Floors,
                Building = o.Building,
                Number = o.Number,
                Square = o.Square,
                Street = o.Street,
                DealTypeId = o.DealTypeId,
                Price = o.Price,
                TypeId = o.TypeId,
                StatusId = o.StatusId
            };
            await _reObjectRepository.AddAsync(obj);
        }

        // Обновление объекта
        public async Task UpdateAsync(CreateREObjectDTO o)
        {
            var obj = await _reObjectRepository.GetByIdAsync(o.Id);
            if (obj != null)
            {
                obj.Rooms = o.Rooms;
                obj.Floors = o.Floors;
                obj.Building = o.Building;
                obj.Number = o.Number;
                obj.Square = o.Square;
                obj.Street = o.Street;
                obj.DealTypeId = o.DealTypeId;
                obj.Price = o.Price;
                obj.TypeId = o.TypeId;
                obj.StatusId = o.StatusId;

                await _reObjectRepository.UpdateAsync(obj);
            }
        }

        // Удаление объекта
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
    }
}
