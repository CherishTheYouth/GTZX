using System.ServiceModel;

namespace WcfContract.ServiceContract
{
    [ServiceContract]
    [Route("File")]
    public interface IFile
    {
        /// <summary>
        /// 删除已上传的文件
        /// </summary>
        /// <param name="path">文件相对路径</param>
        /// <returns>是否删除成功</returns>
        [OperationContract]
        bool DeleteFile(string path);

        /// <summary>
        /// 下载文件
        /// </summary>
        /// <param name="path">文件相对路径</param>
        /// <param name="offset">偏移量</param>
        /// <param name="length">长度</param>
        /// <returns>读取的文件二进制流</returns>
        [OperationContract]
        byte[] DownloadFile(string path, long offset, long length);

        /// <summary>
        /// 上传文件
        /// </summary>
        /// <param name="path">文件相对路径</param>
        /// <param name="offset">偏移量</param>
        /// <param name="bufferBytes">二进制流数据</param>
        /// <returns>上传后的文件大小</returns>
        [OperationContract]
        long UploadFile(string path, long offset, byte[] bufferBytes);

        /// <summary>
        /// 获取已上传文件大小
        /// </summary>
        /// <param name="path">文件相对路径</param>
        /// <returns></returns>
        [OperationContract]
        long GetUploadedLength(string path);

        /// <summary>
        /// 获取文件是否已存在
        /// </summary>
        /// <param name="path">文件相对路径</param>
        /// <returns></returns>
        [OperationContract]
        bool IsExist(string path);

        /// <summary>
        /// 生成缩略图（仅对图片有效）
        /// </summary>
        /// <param name="path">原图路径</param>
        /// <returns>生成的缩略图路径</returns>
        [OperationContract]
        string GenerateShortcut(string path);
    }
}
