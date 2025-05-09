using Application.Common.Models.MajorCategoryModel;

namespace Application.Common.Models.MajorModel
{
    public class MajorResponseModel
    {
        public string Id { get; set; }
        public string MajorCode { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public string SkillYouLearn { get; set; }
        public string MajorCategoryCode { get; set; }
        public MajorCategoryResponseModel? MajorCategory { get; set; }
		public List<SubjectResponse>? Subjects { get; set; } = new List<SubjectResponse>();
	}
    public class SubjectResponse
    {
		public Guid MasterSubjectId { get; set; }
		public string? MasterSubjectName { get; set; }
	}
}
