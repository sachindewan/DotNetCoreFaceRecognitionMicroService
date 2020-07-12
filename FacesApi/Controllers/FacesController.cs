using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using OpenCvSharp;

namespace FacesApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class FacesController : ControllerBase
    {
        //[HttpPost]
        //public async Task<List<Byte[]>> ReadFaces()
        //{
        //    using (var ms = new MemoryStream(2048))
        //    {
        //        await Request.Body.CopyToAsync(ms);
        //        var faces = GetFaces(ms.ToArray());
        //        return faces;
        //    }
        //}
        [HttpPost()]
        public async Task<Tuple<List<Byte[]>,Guid>> ReadFaces(Guid orderId)
        {
            using (var ms = new MemoryStream(2048))
            {
                await Request.Body.CopyToAsync(ms);
                var faces = GetFaces(ms.ToArray());
                return new Tuple<List<byte[]>, Guid>(faces,orderId);
            }
        }
        private List<Byte[]> GetFaces(byte[] image)
        {
            Mat src = Cv2.ImDecode(image, ImreadModes.Color);
            var faceList = new List<byte[]>();
            // convert the byte array in jpeg image and save the image coming from source.
            // in the root directory or the testing purpose
            if (!src.Empty()) {
                src.SaveImage("image.jpg", new ImageEncodingParam(ImwriteFlags.JpegProgressive, 255));
                var file = Path.Combine(Directory.GetCurrentDirectory(), "CascadeFile", "haarcascade_frontalface_default.xml");
                var faceCascade = new CascadeClassifier();
                faceCascade.Load(file);
                var faces = faceCascade.DetectMultiScale(src, 1.1, 6, HaarDetectionType.DoRoughSearch, new Size(60, 60));
               
                int j = 0;
                foreach (var rect in faces)
                {
                    var faceImage = new Mat(src, rect);
                    faceList.Add(faceImage.ToBytes(".jpg"));
                    faceImage.SaveImage("face" + j + ".jpg", new ImageEncodingParam(ImwriteFlags.JpegProgressive, 255));
                    j++;
                }
               
            }
            return faceList;

        }
    }
}
