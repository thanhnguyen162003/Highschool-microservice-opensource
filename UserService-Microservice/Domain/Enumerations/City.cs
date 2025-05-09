using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace Domain.Enumerations
{
    [JsonConverter(typeof(JsonStringEnumConverter))]
    public enum City
    {
        HaNoi,
        DaNang,
        QuyNhon,
        TpHCM,
        CanTho,
        Hue,
        Vinh,
        BacNinh,
        DaLat,
        HaiPhong,
        NhaTrang,
        ThaiNguyen,
        VinhPhuc,
        HungYen,
        NamDinh,
        LongAn,
        BinhDuong,
        ThaiBinh,
        BienHoa
    }
}
