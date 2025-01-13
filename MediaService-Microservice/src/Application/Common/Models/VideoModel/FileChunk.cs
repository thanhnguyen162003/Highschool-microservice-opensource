namespace Application.Common.Models.VideoModel;

public class FileChunk
{
    public string FileName { get; set; }      // Name of the file being uploaded
    public byte[] Data { get; set; }          // Chunk of the file's binary data
    public int ChunkNumber { get; set; }      // Chunk number (if uploading in parts)
    public int TotalChunks { get; set; }      // Total number of chunks (for multi-part uploads)
    public string ContentType { get; set; }   // MIME type of the file (e.g., video/mp4)
}
