using HaloEzAPI.Abstraction.Enum;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HaloEzAPI
{
    public interface IHaloApiService : IHaloAPIServiceCore
    {
        //Profile
        Task<Image> GetProfileEmblem(string gamerTag, int size = 256);
        Task<Image> GetSpartanImage(string gamerTag, int size = 256, CropType cropType = CropType.Full);
    }
}
