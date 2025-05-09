using Domain.Enumerations;

namespace Application.Common.Models.MajorCategoryModel
{
    public class MajorCategoryRequestModel
    {
        public string MajorCategoryCode { get; set; }
        public string Name { get; set; }
        public List<MBTIType> MBTITypes { get; set; }
        public HollandTrait PrimaryHollandTrait { get; set; }
        public HollandTrait SecondaryHollandTrait { get; set; }
    }

}
