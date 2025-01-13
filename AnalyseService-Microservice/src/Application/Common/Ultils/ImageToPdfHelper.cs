// using System;
// using System.Collections.Generic;
// using System.IO;
// using System.Net;
// using System.Threading.Tasks;
// using Application.Common.Models;
// using Application.Common.Models.FileModel;
// using CloudinaryDotNet;
// using CloudinaryDotNet.Actions;
// using Microsoft.AspNetCore.Http;
// using Microsoft.Extensions.Logging;
// using Microsoft.Extensions.Options;
// using iTextSharp.text;
// using iTextSharp.text.pdf;
//
// namespace Application.Common.Ultils
// {
//     public class ImageToPdfHelper
//     {
//         private readonly Cloudinary _cloudinary;
//         private readonly ILogger<ImageToPdfHelper> _logger;
//
//         public ImageToPdfHelper(IOptions<CloudinarySettings> config, ILogger<ImageToPdfHelper> logger)
//         {
//             if (config == null || logger == null)
//             {
//                 throw new ArgumentNullException(config == null ? nameof(config) : nameof(logger));
//             }
//
//             var account = new Account(
//                 config.Value.CloudName,
//                 config.Value.ApiKey,
//                 config.Value.ApiSecret);
//
//             _cloudinary = new Cloudinary(account);
//             _logger = logger;
//         }
//
//         public async Task<ResponseModel> ConvertImagesToPdfAsync(List<IFormFile> imageFiles, string outputPdfName)
//         {
//             if (imageFiles == null || imageFiles.Count == 0)
//             {
//                 _logger.LogWarning("No images provided for conversion.");
//                 return new ResponseModel(HttpStatusCode.BadRequest, "No images to convert.");
//             }
//
//             // Create a memory stream to hold the PDF document in memory
//             await using (var memoryStream = new MemoryStream())
//             {
//                 Document document = null;
//                 PdfWriter pdfWriter = null;
//
//                 try
//                 {
//                     document = new Document(PageSize.A4);
//                     pdfWriter = PdfWriter.GetInstance(document, memoryStream);
//                     pdfWriter.CloseStream = false;
//                     document.Open();
//                     foreach (var imageFile in imageFiles)
//                     {
//                         await using(var imageStream = new MemoryStream())
//                         {
//                             await imageFile.CopyToAsync(imageStream);
//                             imageStream.Position = 0;
//
//                             try
//                             {
//                                 var image = iTextSharp.text.Image.GetInstance(imageStream.ToArray());
//                                 image.ScaleToFit(PageSize.A4.Width, PageSize.A4.Height);
//                                 image.Alignment = iTextSharp.text.Image.UNDERLYING;
//                                 document.Add(image);
//                             }
//                             catch (Exception imgEx)
//                             {
//                                 _logger.LogError($"Error processing image {imageFile.FileName}: {imgEx.Message}");
//                                 return new ResponseModel(HttpStatusCode.BadRequest, $"Failed to process image {imageFile.FileName}.", imgEx.Message);
//                             }
//                         }
//                     }
//
//                     document.Close();
//                     pdfWriter.Close();
//                     
//                 }
//                 catch (Exception pdfEx)
//                 {
//                     _logger.LogError($"Error creating PDF document: {pdfEx.Message}");
//                     return new ResponseModel(HttpStatusCode.BadRequest, "Failed to create PDF document.", pdfEx.Message);
//                 }
//                 memoryStream.Position = 0;
//                 
//                     var uploadParams = new RawUploadParams
//                     {
//                         File = new FileDescription(outputPdfName + ".pdf", memoryStream)
//                     };
//
//                     var uploadResult = await _cloudinary.UploadAsync(uploadParams);
//
//                     if (uploadResult.Error != null)
//                     {
//                         return new ResponseModel(HttpStatusCode.BadRequest, "Failed to upload PDF to Cloudinary.", uploadResult.Error.Message);
//                     }
//
//                     var imagePdf = new ImagePdfModelResponse()
//                     {
//                         Format = uploadResult.Format.ToString(),
//                         ImageUrl = uploadResult.Url.ToString(),
//                         PublicIdUrl = uploadResult.PublicId.ToString()
//                     };
//
//                     return new ResponseModel(HttpStatusCode.OK, "PDF uploaded successfully.", imagePdf);
//                 
//             }
//         }
//     }
// }
