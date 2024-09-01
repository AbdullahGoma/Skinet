using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Core.Entities;
using Core.Interfaces;

namespace Infrastructure.Data
{
    public class DeliveryMethodRepository : BaseRepository<DeliveryMethod>, IDeliveryMethodRepository
    {
        public DeliveryMethodRepository(StoreContext context) : base(context)
        {
        }
    }
}