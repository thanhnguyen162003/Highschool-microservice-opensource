using System.Collections.Generic;

namespace Application.Common.Models.FlashcardFeatureModel;

public class UpdateProgressModel
{
    public Guid FlashcardContentId { get; init; }
    
    /// <summary>
    /// Rating dựa theo FSRS:
    /// 1 = Again (không nhớ)
    /// 2 = Hard (khó nhớ)
    /// 3 = Good (nhớ được sau chút cố gắng)
    /// 4 = Easy (dễ dàng nhớ)
    /// </summary>
    public int Rating { get; init; }
    public double TimeSpent { get; init; }
}

/// <summary>
/// Model chứa danh sách các UpdateProgressModel để gửi kết quả học tập hàng loạt
/// </summary>
public class BatchUpdateProgressModel
{
    public List<UpdateProgressModel> ProgressUpdates { get; init; } = new List<UpdateProgressModel>();
}