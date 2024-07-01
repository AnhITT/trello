using BusinessLogic_Layer.Entity;
using BusinessLogic_Layer.Enums;
using DataAccess_Layer.Interfaces;
using DataAccess_Layer.DTOs;
using DataAccess_Layer.Models;
using Microsoft.Extensions.Localization;
using BusinessLogic_LayerDataAccess_Layer.Common;
using System.Threading.Tasks;

namespace BusinessLogic_Layer.Service
{
    public class UploadService
    {
        private readonly IUnitOfWorkUpload _unitOfWorkUpload;
        public readonly CallApi _callApi;
        private readonly IStringLocalizer<SharedResource> _localizer;

        public UploadService(IUnitOfWorkUpload unitOfWorkUpload, CallApi callApi, IStringLocalizer<SharedResource> localizer)
        {
            _unitOfWorkUpload = unitOfWorkUpload;
            _callApi = callApi;
            _localizer = localizer;
        }
        public async Task<ResultObject> GetAll()
        {
            try
            {
                var result = _unitOfWorkUpload.AttachmentFileRepository.GetAll();
                return new ResultObject
                {
                    Data = result,
                    Success = true,
                    StatusCode = EnumStatusCodesResult.Success
                };
            }
            catch (Exception ex)
            {
                return new ResultObject
                {
                    Message = ex.Message,
                    Success = false,
                    StatusCode = EnumStatusCodesResult.InternalServerError
                };
            }
            finally
            {
                _unitOfWorkUpload.Dispose();
            }
        }

        public async Task<ResultObject> GetFileToTask(Guid idTask)
        {
            try
            {
                var result = _unitOfWorkUpload.AttachmentFileRepository.Find(n => n.TaskId == idTask).ToList();
                return new ResultObject
                {
                    Data = result,
                    Success = true,
                    StatusCode = EnumStatusCodesResult.Success
                };
            }
            catch (Exception ex)
            {
                return new ResultObject
                {
                    Message = ex.Message,
                    Success = false,
                    StatusCode = EnumStatusCodesResult.InternalServerError
                };
            }
            finally
            {
                _unitOfWorkUpload.Dispose();
            }
        }
        public async Task<ResultObject> UploadFileToTask(ApiFileUpload apiFileUpload)
        {
            try
            {
                #region Check Task Card exists
                var checkTask = _callApi.IsNotFindTask(apiFileUpload.TaskId);
                if (checkTask.Result == false)
                    return new ResultObject
                    {
                        Message = $"{_localizer[SharedResourceKeys.TaskCard]} {_localizer[SharedResourceKeys.NotFound]}",
                        Success = true,
                        StatusCode = EnumStatusCodesResult.NotFound
                    };
                #endregion

                var folderName = Path.Combine("FileUpload");
                var path = Path.Combine(Directory.GetCurrentDirectory(), folderName);
                if (!Directory.Exists(path))
                {
                    Directory.CreateDirectory(path);
                }

                List<AttachmentFile> files = new List<AttachmentFile>();
                List<string> fileSystem = new List<string>();
                List<string> fileSuccess = new List<string>();

                foreach (var file in apiFileUpload.Files)
                {
                    FileType fileType;
                    if (Enum.TryParse(Path.GetExtension(file.FileName).ToLower().TrimStart('.'),
                        true, out fileType) && fileType >= FileType.Sys)
                    {
                        fileSystem.Add(file.FileName);
                        continue;
                    }
                    var fileDetails = new AttachmentFile()
                    {
                        FileName = file.FileName,
                        FileType = Path.GetExtension(file.FileName),
                        FileNameToken = Guid.NewGuid().ToString() + Path.GetExtension(file.FileName),
                        TaskId = apiFileUpload.TaskId,
                    };

                    var filePath = Path.Combine(path, fileDetails.FileNameToken.ToString());
                    using (var fileStream = new FileStream(filePath, FileMode.Create))
                    {
                        await file.CopyToAsync(fileStream);
                    }
                    files.Add(fileDetails);
                    fileSuccess.Add(file.FileName);
                }
                _unitOfWorkUpload.AttachmentFileRepository.AddRange(files);
                _unitOfWorkUpload.SaveChanges();

                #region Create text response
                string failedFilesString = string.Empty;
                string successFilesString = string.Empty;
                if (files.Count > 0)
                    successFilesString = $"{_localizer[SharedResourceKeys.UploadSuccess]}: {string.Join(", ", fileSuccess)}";
                else
                    successFilesString = _localizer[SharedResourceKeys.NoSuccessFile];

                if (fileSystem.Count > 0)
                    failedFilesString = $"{_localizer[SharedResourceKeys.UploadFailde]}: {string.Join(", ", fileSystem)}";
                else
                    failedFilesString = _localizer[SharedResourceKeys.NoFaildeFile];
                #endregion

                return new ResultObject
                {
                    Data = $"{successFilesString}. {failedFilesString}",
                    Success = true,
                    StatusCode = EnumStatusCodesResult.Success
                };
            }
            catch (Exception ex)
            {
                return new ResultObject
                {
                    Message = ex.Message,
                    Success = false,
                    StatusCode = EnumStatusCodesResult.InternalServerError
                };
            }
            finally
            {
                _unitOfWorkUpload.Dispose();
            }
        }
        public async Task<DownloadDTO> DownloadFile(Guid Id)
        {
            try
            {
                //Check File exists
                var checkFile = _unitOfWorkUpload.AttachmentFileRepository.FirstOrDefault(x => x.Id == Id);
                if (checkFile == null)
                    return null;

                var filePath = Path.Combine(
                   Directory.GetCurrentDirectory(), "FileUpload",
                   checkFile.FileNameToken);
                if (!System.IO.File.Exists(filePath))
                {
                    return null;
                }

                var fileDownload = new DownloadDTO()
                {
                    FileBytes = await File.ReadAllBytesAsync(filePath),
                    FileName = checkFile.FileName
                };
                return fileDownload;
            }
            catch (Exception ex)
            {
                return null;
            }
            finally
            {
                _unitOfWorkUpload.Dispose();
            }
        }
        public async Task<ResultObject> DeleteFile(Guid id)
        {
            try
            {
                #region Check File exists
                var checkFile = _unitOfWorkUpload.AttachmentFileRepository.FirstOrDefault(x => x.Id == id);
                if (checkFile == null)
                    return new ResultObject
                    {
                        Message = _localizer[SharedResourceKeys.NotFound],
                        Success = false,
                        StatusCode = EnumStatusCodesResult.NotFound
                    };
                #endregion

                var filePath = Path.Combine(
                   Directory.GetCurrentDirectory(), "FileUpload",
                   checkFile.FileNameToken);
                if (System.IO.File.Exists(filePath))
                {
                    System.IO.File.Delete(filePath);
                }

                _unitOfWorkUpload.AttachmentFileRepository.Remove(checkFile);
                _unitOfWorkUpload.SaveChanges();

                return new ResultObject
                {
                    Message = _localizer[SharedResourceKeys.DeleteSuccess],
                    Success = true,
                    StatusCode = EnumStatusCodesResult.Success
                };
            }
            catch (Exception ex)
            {
                return new ResultObject
                {
                    Message = ex.Message,
                    Success = false,
                    StatusCode = EnumStatusCodesResult.InternalServerError
                };
            }
            finally
            {
                _unitOfWorkUpload.Dispose();
            }
        }
    }
}
