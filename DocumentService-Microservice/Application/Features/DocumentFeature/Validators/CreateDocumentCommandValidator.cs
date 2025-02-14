﻿using Application.Common.Models.DocumentModel;
using Application.Messages.Document;

namespace Application.Features.DocumentFeature.Validators
{
    public class CreateDocumentCommandValidator : AbstractValidator<CreateDocumentRequestModel>
    {
        public CreateDocumentCommandValidator()
        {
            RuleFor(request => request.DocumentName)
                .NotEmpty().WithMessage("Tên của tài liệu không được bỏ trống")
                .MaximumLength(255).WithMessage(DocumentMessage.DOCUMENT_NAME_INVALID_MAX_LENGTH);

            RuleFor(request => request.DocumentDescription)
                .MaximumLength(1000).WithMessage(DocumentMessage.DOCUMENT_DESCRIPTION_INVALID_MAX_LENGTH);
            
            RuleFor(request => request.DocumentYear)
                .LessThanOrEqualTo((int)DateTime.Now.Year)
                .WithMessage(DocumentMessage.DOCUMENT_YEAR_INVALID_MAX)
                .GreaterThanOrEqualTo(1960)
                .WithMessage(DocumentMessage.DOCUMENT_YEAR_INVALID_MIN);

            RuleFor(request => request.Semester)
                .GreaterThanOrEqualTo(1).WithMessage("Học kỳ không hợp lệ")
                .LessThanOrEqualTo(3).WithMessage("Học kỳ không hợp lệ");

            //RuleFor(request => request.Author)
            //    .MaximumLength(255).WithMessage(DocumentMessage.DOCUMENT_AUTHOR_INVALID_MAX_LENGTH);
        }
    }
}
