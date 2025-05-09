using Application.Common.Models.Common;

namespace Application.Features.GetCreatorFlashcard
{
	public class GetCreatorFlashcardQuery : IRequest<ResponseModel>
	{
		public Guid UserId { get; set; }
	}
}
