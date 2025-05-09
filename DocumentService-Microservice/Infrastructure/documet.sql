CREATE TABLE IF NOT EXISTS "Category" (
                                          id uuid NOT NULL,
                                          "categoryName" character varying(255) NOT NULL,
    "categorySlug" character varying(255) NOT NULL,
    PRIMARY KEY(id)
    );


CREATE TABLE IF NOT EXISTS "Chapter" (
                                         id uuid NOT NULL,
                                         "subjectCurriculumId" uuid NOT NULL,
                                         "chapterName" character varying(1000) NOT NULL,
    "chapterLevel" character varying(50),
    "semester" character varying(50),
    description character varying(1000),
    "like" integer,
    "createdAt" timestamp with time zone DEFAULT CURRENT_TIMESTAMP NOT NULL,
    "updatedAt" timestamp with time zone,
    "deletedAt" timestamp with time zone,
    PRIMARY KEY(id)
    );

CREATE TABLE "Curriculum" (
                              id UUID PRIMARY KEY not null,
                              "curriculumName" VARCHAR(1000),
                              "createdAt" timestamp with time zone DEFAULT CURRENT_TIMESTAMP NOT NULL,
                              "updatedAt" timestamp with time zone,
                              "deletedAt" timestamp with time zone
);

CREATE TABLE IF NOT EXISTS "Document" (
                                          id uuid NOT NULL,
                                          "documentName" character varying(255) NOT NULL,
    "documentDescription" character varying(1000),
    "documentYear" integer,
    "view" integer,
    slug character varying(255),
    download integer,
    "like" integer,
    "createdAt" timestamp with time zone DEFAULT CURRENT_TIMESTAMP NOT NULL,
    "updatedAt" timestamp with time zone,
    "createdBy" uuid,
    "updatedBy" uuid,
    "deletedAt" timestamp with time zone,
    "schoolId" uuid,
    "subjectCurriculumId" uuid,
    "semester" integer,
    PRIMARY KEY(id)
    );


CREATE TABLE IF NOT EXISTS "Enrollment" (
                                            id uuid NOT NULL,
                                            "baseUserId" uuid NOT NULL,
                                            "subjectCurriculumId" uuid NOT NULL,
                                            "updatedAt" timestamp with time zone,
                                            "createdAt" timestamp with time zone DEFAULT CURRENT_TIMESTAMP NOT NULL,
                                            PRIMARY KEY(id)
    );


CREATE TABLE IF NOT EXISTS "EnrollmentProgress" (
                                                    id uuid NOT NULL,
                                                    "enrollmentId" uuid NOT NULL,
                                                    "lessonId" uuid NOT NULL,
                                                    "updatedAt" timestamp with time zone,
                                                    "createdAt" timestamp with time zone DEFAULT CURRENT_TIMESTAMP NOT NULL,
                                                    PRIMARY KEY(id)
    );


CREATE TABLE IF NOT EXISTS "Exam" (
                                      id uuid NOT NULL,
                                      "subjectId" uuid NOT NULL,
                                      "examName" character varying(255) NOT NULL,
    "examDescription" character varying(255),
    "examCode" character varying(50) NOT NULL,
    "view" integer,
    slug character varying(200),
    "examYear" character varying(4),
    author character varying(100),
    grade integer,
    "type" character varying(50),
    page integer,
    download integer,
    "like" integer,
    "createdAt" timestamp with time zone DEFAULT CURRENT_TIMESTAMP NOT NULL,
    "updatedAt" timestamp with time zone,
    "deletedAt" timestamp with time zone,
    PRIMARY KEY(id)
    );


CREATE TABLE IF NOT EXISTS "ExamAnswer" (
                                            id uuid NOT NULL,
                                            "examId" uuid NOT NULL,
                                            "createdAt" timestamp with time zone DEFAULT CURRENT_TIMESTAMP NOT NULL,
                                            "updatedAt" timestamp with time zone,
                                            "createdBy" character varying(50),
    "updatedBy" character varying(50),
    "deletedAt" timestamp with time zone,
    PRIMARY KEY(id)
    );


CREATE TABLE IF NOT EXISTS "Flashcard" (
                                           id uuid NOT NULL,
                                           "userId" uuid NOT NULL,
                                           "subjectId" uuid NOT NULL,
                                           "flashcardName" character varying(255) NOT NULL,
    slug character varying(200) NOT NULL,
    "flashcardDescription" character varying(255),
    status character varying(50) NOT NULL,
    "vote" integer,
    star double precision,
    "isArtificalIntelligence" boolean,
    "totalView" integer,
    "todayView" integer
    "createdAt" timestamp with time zone DEFAULT CURRENT_TIMESTAMP NOT NULL,
    "updatedAt" timestamp with time zone,
    "createdBy" character varying(50),
    "updatedBy" character varying(50),
    "deletedAt" timestamp with time zone,
    created boolean,
    PRIMARY KEY(id)
    );


CREATE TABLE IF NOT EXISTS "FlashcardContent" (
                                                  id uuid NOT NULL,
                                                  "flashcardId" uuid NOT NULL,
                                                  "flashcardContentTerm" character varying(1000) NOT NULL,
    "flashcardContentDefinition" character varying,
    image character varying(300),
    status character varying(50),
    "createdAt" timestamp with time zone DEFAULT CURRENT_TIMESTAMP NOT NULL,
    "updatedAt" timestamp with time zone,
    "createdBy" character varying(50),
    "updatedBy" character varying(50),
    "deletedAt" timestamp with time zone,
    "flashcardContentTermRichText" character varying,
    rank integer,
    "flashcardContentDefinitionRichText" character varying,
    PRIMARY KEY(id)
    );

CREATE TABLE IF NOT EXISTS "Lesson" (
                                        id uuid NOT NULL,
                                        "chapterId" uuid NOT NULL,
                                        "lessonName" character varying(1000) NOT NULL,
    "lessonBody" character varying,
     "youtubeVideoUrl" character varying,
    "lessonMaterial" VARCHAR(3600),
    slug character varying(1000) NOT NULL,
    "like" integer,
    "displayOrder" integer,
    "createdAt" timestamp with time zone DEFAULT CURRENT_TIMESTAMP NOT NULL,
    "updatedAt" timestamp with time zone,
    "deletedAt" timestamp with time zone,
    "videoUrl" character varying(1000),
    PRIMARY KEY(id)
    );


CREATE TABLE IF NOT EXISTS "Province" (
                                          id integer NOT NULL,
                                          "provinceName" character varying(1000),
    "createdAt" timestamp with time zone,
    "updatedAt" timestamp with time zone,
    "deletedAt" timestamp with time zone,
    PRIMARY KEY(id)
    );

CREATE TABLE IF NOT EXISTS "RecommendedData" (
                                                 id uuid NOT NULL,
                                                 "userId" uuid NOT NULL,
                                                 "subjectIds" character varying NOT NULL,
                                                 "typeExam" character varying,
                                                 grade INT,
                                                 "objectId" character varying,
                                                 PRIMARY KEY(id)
    );


CREATE TABLE IF NOT EXISTS "School" (
                                        id uuid NOT NULL,
                                        "schoolName" character varying(1000),
    "provinceId" integer,
    "locationDetail" character varying(1000),
    "createdAt" timestamp with time zone,
    "updatedAt" timestamp with time zone,
    "deletedAt" timestamp with time zone,
    PRIMARY KEY(id)
    );


CREATE TABLE IF NOT EXISTS "Subject" (
                                         id uuid NOT NULL,
                                         "subjectName" character varying(1000) NOT NULL,
    "subjectDescription" character varying(1000),
    information character varying(1000),
    slug character varying(1000) NOT NULL,
    image character varying(255),
    "like" integer,
    "view" integer,
    "createdAt" timestamp with time zone DEFAULT CURRENT_TIMESTAMP NOT NULL,
    "updatedAt" timestamp with time zone,
    "deletedAt" timestamp with time zone,
    "subjectCode" character varying(100),
    "categoryId" uuid,
    PRIMARY KEY(id)
    );

CREATE TABLE IF NOT EXISTS "Theory" (
                                        id uuid NOT NULL,
                                        "lessonId" uuid NOT NULL,
                                        "theoryName" character varying(1000) NOT NULL,
    "theoryDescription" character varying(1000),
    "theoryContentJson" character varying,
    "theoryContentHtml" character varying,
    "createdAt" timestamp with time zone DEFAULT CURRENT_TIMESTAMP NOT NULL,
    "updatedAt" timestamp with time zone,
    "deletedAt" timestamp with time zone,
    PRIMARY KEY(id)
    );




CREATE TABLE "SubjectCurriculum" (
                                     id UUID PRIMARY KEY,
                                     "subjectCurriculumName" VARCHAR(1000),
                                     "subjectId" UUID NOT NULL,
                                     "curriculumId" UUID NOT NULL,
                                     "isPublish" bool,
                                     FOREIGN KEY ("subjectId") REFERENCES "Subject"(id) ON DELETE SET NULL,
                                     FOREIGN KEY ("curriculumId") REFERENCES "Curriculum"(id) ON DELETE SET NULL
);
CREATE TABLE "UserLike" (
                                     id UUID PRIMARY KEY,
                                     "userId" UUID NOT NULL,
                                     "documentId" UUID,
                                     "flashcardId" UUID,
                                     "subjectId" UUID,
                                     "lessonId" UUID,
                                     "createdAt" TIMESTAMP DEFAULT CURRENT_TIMESTAMP,
                                     "updatedAt" TIMESTAMP,
                                     FOREIGN KEY ("subjectId") REFERENCES "Subject"(id) ON DELETE SET NULL,
                                     FOREIGN KEY ("documentId") REFERENCES "Document"(id) ON DELETE SET NULL,
                                     FOREIGN KEY ("flashcardId") REFERENCES "Flashcard"(id) ON DELETE SET NULL,
                                     FOREIGN KEY ("lessonId") REFERENCES "Lesson"(id) ON DELETE SET NULL
);

CREATE TABLE "UserFlashcardProgress" (
    "id" UUID PRIMARY KEY,
    "userId" UUID NOT NULL,
    "flashcardContentId" UUID NOT NULL,
    "difficulty" DOUBLE PRECISION,
    "stability" DOUBLE PRECISION,
    "state" TEXT,
    "dueDate" TIMESTAMP WITH TIME ZONE,
    "lastReviewDate" TIMESTAMP WITH TIME ZONE,
    "timeSpent" DOUBLE PRECISION,
    CONSTRAINT "fk_flashcardContent" FOREIGN KEY ("flashcardContentId") REFERENCES "FlashcardContent" ("id") ON DELETE CASCADE
);

CREATE TABLE "Question" (
    "id" UUID PRIMARY KEY,
    "questionContent" TEXT NOT NULL,
    "lessonId" UUID,
    "chapterId" UUID,
    "subjectCurriculumId" UUID,
    "subjectId" UUID,
    "difficulty" VARCHAR(50) NOT NULL,
    "questionType" VARCHAR(50) NOT NULL,
    "createdBy" UUID NOT NULL,
    "updatedBy" UUID NOT NULL,
    "createdAt" TIMESTAMPTZ DEFAULT CURRENT_TIMESTAMP,
    "updatedAt" TIMESTAMPTZ DEFAULT CURRENT_TIMESTAMP,
    
    -- Quan hệ tới các bảng khác
    CONSTRAINT "fk_question_lesson" FOREIGN KEY ("lessonId") REFERENCES "Lesson"("id") ON DELETE CASCADE,
    CONSTRAINT "fk_question_chapter" FOREIGN KEY ("chapterId") REFERENCES "Chapter"("id") ON DELETE CASCADE,
    CONSTRAINT "fk_question_subject_curriculum" FOREIGN KEY ("subjectCurriculumId") REFERENCES "SubjectCurriculum"("id") ON DELETE CASCADE,
    CONSTRAINT "fk_question_subject" FOREIGN KEY ("subjectId") REFERENCES "Subject"("id") ON DELETE CASCADE
);

CREATE TABLE "QuestionAnswer" (
    "id" UUID PRIMARY KEY,
    "questionId" UUID NOT NULL,
    "answerContent" TEXT NOT NULL,
    "isCorrectAnswer" BOOLEAN NOT NULL,
    "createdBy" UUID NOT NULL,
    "updatedBy" UUID NOT NULL,
    "createdAt" TIMESTAMPTZ DEFAULT CURRENT_TIMESTAMP,
    "updatedAt" TIMESTAMPTZ DEFAULT CURRENT_TIMESTAMP,
    CONSTRAINT "fk_question_answer_question" FOREIGN KEY ("questionId") REFERENCES "Question"("id") ON DELETE CASCADE
);

CREATE TABLE "UserQuizProgress" (
    "id" UUID PRIMARY KEY,
    "userId" UUID NOT NULL,
    "lessonId" UUID,
    "chapterId" UUID,
    "subjectCurriculumId" UUID,
    "subjectId" UUID,
    "questionIds" TEXT[], -- Sử dụng mảng TEXT để lưu danh sách các ID câu hỏi (mảng kiểu string trong C#)
    "recognizingQuestionQuantity" INT NOT NULL,
    "recognizingPercent" FLOAT,
    "comprehensingQuestionQuantity" INT NOT NULL,
    "comprehensingPercent" FLOAT,
    "lowLevelApplicationQuestionQuantity" INT NOT NULL,
    "lowLevelApplicationPercent" FLOAT,
    "highLevelApplicationQuestionQuantity" INT NOT NULL,
    "highLevelApplicationPercent" FLOAT,
    "createdAt" TIMESTAMPTZ DEFAULT CURRENT_TIMESTAMP,
    "updatedAt" TIMESTAMPTZ DEFAULT CURRENT_TIMESTAMP,

    -- Quan hệ tới các bảng khác
    CONSTRAINT "fk_user_quiz_progress_lesson" FOREIGN KEY ("lessonId") REFERENCES "Lesson"("id") ON DELETE CASCADE,
    CONSTRAINT "fk_user_quiz_progress_chapter" FOREIGN KEY ("chapterId") REFERENCES "Chapter"("id") ON DELETE CASCADE,
    CONSTRAINT "fk_user_quiz_progress_subject_curriculum" FOREIGN KEY ("subjectCurriculumId") REFERENCES "SubjectCurriculum"("id") ON DELETE CASCADE,
    CONSTRAINT "fk_user_quiz_progress_subject" FOREIGN KEY ("subjectId") REFERENCES "Subject"("id") ON DELETE CASCADE
);

create table "FolderUser"
(
    id          uuid not null
        constraint folderuser_pk
            primary key,
    name        varchar,
    "userId"    uuid,
    "createdAt" timestamp
);

create table "FlashcardFolder"
(
    "flashcardId" uuid
        constraint flashcardfolder_flashcard_id_fk
            references "Flashcard",
    "folderId"    uuid
        constraint flashcardfolder_folderuser_id_fk
            references "FolderUser",
    id            uuid not null
        constraint flashcardfolder_pk
            primary key,
    "createdAt"   timestamp
);



create table "DocumentFolder"
(
    id           uuid not null
        constraint documentfolder_pk
            primary key,
    "documentId" uuid
        constraint documentfolder_document_id_fk
            references "Document",
    "folderId"   uuid
        constraint documentfolder_folderuser_id_fk
            references "FolderUser",
    "createdAt"  timestamp
);

CREATE TABLE "Container" (
    "id" UUID PRIMARY KEY,
    "userId" UUID NOT NULL,
    "viewAt" TIMESTAMP NOT NULL,
    "shuffleFlashcards" BOOLEAN NOT NULL,
    "learnRound" INT,
    "learnMode" VARCHAR(100) NOT NULL,
    "shuffleLearn" BOOLEAN NOT NULL,
    "studyStarred" BOOLEAN NOT NULL,
    "answerWith" VARCHAR(100) NOT NULL,
    "multipleAnswerMode" VARCHAR(100) NOT NULL,
    "extendedFeedbackBank" BOOLEAN NOT NULL,
    "enableCardsSorting" BOOLEAN NOT NULL,
    "cardsRound" INT,
    "cardsStudyStarred" BOOLEAN NOT NULL,
    "cardsAnswerWith" VARCHAR(100) NOT NULL,
    "matchStudyStarred" BOOLEAN NOT NULL,
    "retrievability" DOUBLE PRECISION NOT NULL DEFAULT 0.9,
    "fsrsParameters" DOUBLE PRECISION[] NOT NULL DEFAULT ARRAY[0.40255, 1.18385, 3.173, 15.69105, 7.1949, 0.5345, 1.4604, 0.0046, 
1.54575, 0.1192, 1.01925, 1.9395, 0.11, 0.29605, 2.2698, 0.2315, 
2.9898, 0.51655, 0.6621]
);

CREATE TABLE "StarredTerm" (
    "userId" UUID NOT NULL,
    "flashcardContentId" UUID NOT NULL,
    "containerId" UUID NOT NULL,
    PRIMARY KEY ("userId", "flashcardContentId"),
    FOREIGN KEY ("containerId") REFERENCES "Container" ("id") ON DELETE CASCADE,
    FOREIGN KEY ("flashcardContentId") REFERENCES "FlashcardContent" ("id") ON DELETE CASCADE
);

CREATE TABLE "StudiableTerm" (
    "id" UUID PRIMARY KEY,
    "mode" VARCHAR(500),
    "userId" UUID NOT NULL,
    "containerId" UUID NOT NULL,
    "flashcardContentId" UUID,
    FOREIGN KEY ("containerId") REFERENCES "Container" ("id") ON DELETE CASCADE,
    FOREIGN KEY ("flashcardContentId") REFERENCES "FlashcardContent" ("id") ON DELETE CASCADE
);

CREATE TABLE "Tag" (
    "Id" UUID PRIMARY KEY,
    "Name" VARCHAR(100) NOT NULL,
    "NormalizedName" VARCHAR(100) NOT NULL,
    "CreatedAt" TIMESTAMP WITH TIME ZONE NOT NULL,
    "UpdatedAt" TIMESTAMP WITH TIME ZONE NOT NULL
);

CREATE TABLE "FlashcardTag" (
    "FlashcardId" UUID NOT NULL,
    "TagId" UUID NOT NULL,
    "CreatedAt" TIMESTAMP WITH TIME ZONE NOT NULL,
    "UpdatedAt" TIMESTAMP WITH TIME ZONE NOT NULL,
    PRIMARY KEY ("FlashcardId", "TagId"),
    CONSTRAINT "FK_FlashcardTag_Flashcard_FlashcardId" FOREIGN KEY ("FlashcardId") REFERENCES "Flashcard" ("id") ON DELETE CASCADE,
    CONSTRAINT "FK_FlashcardTag_Tag_TagId" FOREIGN KEY ("TagId") REFERENCES "Tag" ("Id") ON DELETE CASCADE
);

CREATE INDEX "IX_Tag_NormalizedName" ON "Tag" ("NormalizedName");
CREATE INDEX "IX_FlashcardTag_TagId" ON "FlashcardTag" ("TagId");

CREATE OR REPLACE FUNCTION normalize_vietnamese(input_text text)
RETURNS text AS $$
DECLARE
    result text := input_text;
BEGIN
    -- Chuyển đổi chữ có dấu thành không dấu
    result := translate(result, 'áàảãạâấầẩẫậăắằẳẵặđéèẻẽẹêếềểễệíìỉĩịóòỏõọôốồổỗộơớờởỡợúùủũụưứừửữựýỳỷỹỵ',
                                'aaaaaaaaaaaaaaaaadeeeeeeeeeeeiiiiioooooooooooooooouuuuuuuuuuuyyyyy');
    
    -- Chuyển đổi chữ hoa có dấu thành không dấu
    result := translate(result, 'ÁÀẢÃẠÂẤẦẨẪẬĂẮẰẲẴẶĐÉÈẺẼẸÊẾỀỂỄỆÍÌỈĨỊÓÒỎÕỌÔỐỒỔỖỘƠỚỜỞỠỢÚÙỦŨỤƯỨỪỬỮỰÝỲỶỸỴ',
                                'AAAAAAAAAAAAAAAAADEEEEEEEEEEEIIIIIOOOOOOOOOOOOOOOOUUUUUUUUUUUYYYYY');
    
    -- Chuyển tất cả về chữ thường
    result := lower(result);
    
    RETURN result;
END;
$$ LANGUAGE plpgsql IMMUTABLE;



ALTER TABLE IF EXISTS "Chapter"
    ADD CONSTRAINT "Chapter_subjectCurriculumId_fkey"
    FOREIGN KEY ("subjectCurriculumId")
    REFERENCES "SubjectCurriculum" (id);

ALTER TABLE IF EXISTS "Document"
    ADD CONSTRAINT "Document_subjectCurriculumId_fkey"
    FOREIGN KEY ("subjectCurriculumId")
    REFERENCES "SubjectCurriculum" (id);

ALTER TABLE IF EXISTS "Document"
    ADD CONSTRAINT "Document_schoolId_fkey"
    FOREIGN KEY ("schoolId")
    REFERENCES "School" (id);

ALTER TABLE IF EXISTS "Enrollment"
    ADD CONSTRAINT "Enrollment_subjectCurriculuimId_fkey"
    FOREIGN KEY ("subjectCurriculumId")
    REFERENCES "SubjectCurriculum" (id)
    ON DELETE CASCADE;

ALTER TABLE IF EXISTS "EnrollmentProgress"
    ADD CONSTRAINT "EnrollmentProgress_enrollmentId_fkey"
    FOREIGN KEY ("enrollmentId")
    REFERENCES "Enrollment" (id)
    ON DELETE CASCADE;

ALTER TABLE IF EXISTS "EnrollmentProgress"
    ADD CONSTRAINT "EnrollmentProgress_lessonId_fkey"
    FOREIGN KEY ("lessonId")
    REFERENCES "Lesson" (id)
    ON DELETE SET NULL;

ALTER TABLE IF EXISTS "Exam"
    ADD CONSTRAINT "Exam_subjectId_fkey"
    FOREIGN KEY ("subjectId")
    REFERENCES "Subject" (id);

ALTER TABLE IF EXISTS "ExamAnswer"
    ADD CONSTRAINT "ExamAnswer_examId_fkey"
    FOREIGN KEY ("examId")
    REFERENCES "Exam" (id);

ALTER TABLE IF EXISTS "FlashcardContent"
    ADD CONSTRAINT "FlashcardContent_flashcardId_fkey"
    FOREIGN KEY ("flashcardId")
    REFERENCES "Flashcard" (id);

ALTER TABLE IF EXISTS "FlashcardTheory"
    ADD CONSTRAINT "FlashcardTheory_theoryId_fkey"
    FOREIGN KEY ("theoryId")
    REFERENCES "Theory" (id);

ALTER TABLE IF EXISTS "Lesson"
    ADD CONSTRAINT "Lesson_chapterId_fkey"
    FOREIGN KEY ("chapterId")
    REFERENCES "Chapter" (id);

ALTER TABLE IF EXISTS "School"
    ADD CONSTRAINT "School_provinceId_fkey"
    FOREIGN KEY ("provinceId")
    REFERENCES "Province" (id)
    ON DELETE SET NULL;

ALTER TABLE IF EXISTS "Subject"
    ADD CONSTRAINT "Subject_categoryId_fkey"
    FOREIGN KEY ("categoryId")
    REFERENCES "Category" (id);

--consider rm this
ALTER TABLE IF EXISTS "Flashcard"
    ADD CONSTRAINT "Flashcard_subjectId_fkey"
    FOREIGN KEY ("subjectId")
    REFERENCES "Subject" (id);

ALTER TABLE IF EXISTS "Theory"
    ADD CONSTRAINT "Theory_lessonId_fkey"
    FOREIGN KEY ("lessonId")
    REFERENCES "Lesson" (id);

INSERT INTO "Category" (id, "categoryName", "categorySlug") VALUES ('0191de97-4f56-7c63-70a6-e9159f732be4', 'Grade 10', 'grade-10');
INSERT INTO "Category" (id, "categoryName", "categorySlug") VALUES ('0191de97-4f5f-70af-175e-b6ad767b45fa', 'DGNL', 'dgnl');
INSERT INTO "Category" (id, "categoryName", "categorySlug") VALUES ('0191de97-4f5f-783a-e370-92657ce1afe1', 'Grade 12', 'grade-12');
INSERT INTO "Category" (id, "categoryName", "categorySlug") VALUES ('0191de97-4f5f-7ce3-c24a-185cf51de9ef', 'THPTQG', 'thptqg');
INSERT INTO "Category" (id, "categoryName", "categorySlug") VALUES ('0191de97-4f5f-7d64-f61e-2b89e43c4f1c', 'Grade 11', 'grade-11');

INSERT INTO "Curriculum"(
    id, "curriculumName", "createdAt", "updatedAt", "deletedAt")
VALUES ('01931f33-7d05-758c-9580-aa477f5249a4', 'Chân trời sáng tạo', '2024-11-12 07:08:08.585207', '2024-11-12 07:08:08.585207', null);

INSERT INTO "Curriculum"(
    id, "curriculumName", "createdAt", "updatedAt", "deletedAt")
VALUES ('01932971-1395-78b7-8033-cd035de9566b', 'Kết nối tri thức', '2024-11-14 06:51:36.977332', '2024-11-14 06:51:36.977332', null);

INSERT INTO "Curriculum"(
    id, "curriculumName", "createdAt", "updatedAt", "deletedAt")
VALUES ('01932971-336a-747b-d6c8-a28afe4411ba', 'Cánh diều', '2024-11-14 06:51:44.875666', '2024-11-14 06:51:44.875666', null);

-- 1. Thêm các cột mới vào bảng Flashcard
ALTER TABLE "Flashcard"
ADD COLUMN "lessonId" UUID NULL,
ADD COLUMN "chapterId" UUID NULL,
ADD COLUMN "subjectCurriculumId" UUID NULL,
ADD COLUMN "flashcardType" VARCHAR(50) NULL;

-- 2. Tạo các ràng buộc khóa ngoại cho các mối quan hệ mới, với tên giống trong DocumentDbContext
ALTER TABLE "Flashcard"
ADD CONSTRAINT "fk_flashcard_lesson" FOREIGN KEY ("lessonId")
    REFERENCES "Lesson" ("id") ON DELETE CASCADE;

ALTER TABLE "Flashcard"
ADD CONSTRAINT "fk_flashcard_chapter" FOREIGN KEY ("chapterId")
    REFERENCES "Chapter" ("id") ON DELETE CASCADE;

ALTER TABLE "Flashcard"
ADD CONSTRAINT "fk_flashcard_subject_curriculum" FOREIGN KEY ("subjectCurriculumId")
    REFERENCES "SubjectCurriculum" ("id") ON DELETE CASCADE;

-- 3. Tạo các index để tối ưu hiệu suất truy vấn
CREATE INDEX "IX_Flashcard_LessonId" ON "Flashcard" ("lessonId");
CREATE INDEX "IX_Flashcard_ChapterId" ON "Flashcard" ("chapterId");
CREATE INDEX "IX_Flashcard_SubjectCurriculumId" ON "Flashcard" ("subjectCurriculumId");
CREATE INDEX "IX_Flashcard_FlashcardType" ON "Flashcard" ("flashcardType");

-- 4. Tạo index tổng hợp cho việc truy vấn theo nhiều điều kiện
CREATE INDEX "IX_Flashcard_Relations" ON "Flashcard" 
("lessonId", "chapterId", "subjectCurriculumId", "subjectId", "flashcardType");

-- 5. Tạo ràng buộc kiểm tra để đảm bảo chỉ một trong các trường được thiết lập
ALTER TABLE "Flashcard"
ADD CONSTRAINT "CK_Flashcard_SingleRelation" CHECK (
    (CASE WHEN "lessonId" IS NOT NULL THEN 1 ELSE 0 END +
     CASE WHEN "chapterId" IS NOT NULL THEN 1 ELSE 0 END +
     CASE WHEN "subjectCurriculumId" IS NOT NULL THEN 1 ELSE 0 END +
     CASE WHEN "subjectId" IS NOT NULL THEN 1 ELSE 0 END) <= 1
);

-- 6. Tạo ràng buộc kiểm tra để đảm bảo FlashcardType phù hợp với ID được chọn
ALTER TABLE "Flashcard"
ADD CONSTRAINT "CK_Flashcard_TypeMatchesId" CHECK (
    ("flashcardType" = 'Lesson' AND "lessonId" IS NOT NULL) OR
    ("flashcardType" = 'Chapter' AND "chapterId" IS NOT NULL) OR
    ("flashcardType" = 'Subject' AND "subjectId" IS NOT NULL) OR
    ("flashcardType" = 'SubjectCurriculum' AND "subjectCurriculumId" IS NOT NULL) OR
    ("flashcardType" IS NULL)
);

-- 7. Cập nhật các Flashcard hiện tại để có FlashcardType dựa trên SubjectId
UPDATE "Flashcard"
SET "flashcardType" = 'Subject'
WHERE "subjectId" IS NOT NULL AND "flashcardType" IS NULL;


-- Tạo index cho các trường FSRS để tối ưu truy vấn
CREATE INDEX IF NOT EXISTS "IX_UserFlashcardProgress_DueDate" ON "UserFlashcardProgress" ("DueDate");
CREATE INDEX IF NOT EXISTS "IX_UserFlashcardProgress_State" ON "UserFlashcardProgress" ("State");

ALTER TABLE public."UserFlashcardProgress"
DROP COLUMN "flashcardId";

-- Step 3: Update existing columns with default values if they don't exist
ALTER TABLE public."UserFlashcardProgress"
ADD COLUMN IF NOT EXISTS "difficulty" DOUBLE PRECISION NOT NULL DEFAULT 5.0,
ADD COLUMN IF NOT EXISTS "stability" DOUBLE PRECISION NOT NULL DEFAULT 0.0,
ADD COLUMN IF NOT EXISTS "state" VARCHAR(20) NOT NULL DEFAULT 'New',
ADD COLUMN IF NOT EXISTS "dueDate" TIMESTAMP WITH TIME ZONE,
ADD COLUMN IF NOT EXISTS "lastReviewDate" TIMESTAMP WITH TIME ZONE;
ADD COLUMN IF NOT EXISTS "timeSpent" DOUBLE PRECISION;

-- Step 4: Update constraints and descriptions
COMMENT ON COLUMN public."UserFlashcardProgress"."State" IS 'Possible values: New, Learning, Review, Relearning';

ALTER TABLE "UserFlashcardProgress"
ADD CONSTRAINT "FK_UserFlashcardProgress_FlashcardContent"
FOREIGN KEY ("flashcardContentId")
REFERENCES "FlashcardContent"("id")
ON DELETE CASCADE;

-- Thêm các cột FSRS mới vào bảng Container
ALTER TABLE "Container"
ADD COLUMN IF NOT EXISTS "retrievability" DOUBLE PRECISION NOT NULL DEFAULT 0.9,
ADD COLUMN IF NOT EXISTS "fsrsParameters" DOUBLE PRECISION[] NOT NULL DEFAULT ARRAY[0.40255, 1.18385, 3.173, 15.69105, 7.1949, 0.5345, 1.4604, 0.0046, 
1.54575, 0.1192, 1.01925, 1.9395, 0.11, 0.29605, 2.2698, 0.2315, 
2.9898, 0.51655, 0.6621];

CREATE TABLE "FSRSPreset" (
    "id" UUID PRIMARY KEY,
    "userId" UUID NOT NULL,
    "fsrsParameters" DOUBLE PRECISION[] NOT NULL DEFAULT ARRAY[0.40255, 1.18385, 3.173, 15.69105, 7.1949, 0.5345, 1.4604, 0.0046, 
1.54575, 0.1192, 1.01925, 1.9395, 0.11, 0.29605, 2.2698, 0.2315, 
2.9898, 0.51655, 0.6621],
    "retrievability" DOUBLE PRECISION NOT NULL DEFAULT 0.9,
    "isPublicPreset" BOOLEAN NOT NULL DEFAULT FALSE,
    "userId" VARCHAR NOT NULL,
    "createdAt" timestamp with time zone DEFAULT CURRENT_TIMESTAMP NOT NULL,
    "updatedAt" timestamp with time zone,
);

-- Tạo index để tối ưu truy vấn
CREATE INDEX "IX_FSRSPreset_UserId" ON "FSRSPreset" ("userId");
CREATE INDEX "IX_FSRSPreset_IsPublicPreset" ON "FSRSPreset" ("isPublicPreset");

-- Thêm cột lastReviewDateHistory vào bảng UserFlashcardProgress
ALTER TABLE "UserFlashcardProgress"
ADD COLUMN IF NOT EXISTS "lastReviewDateHistory" double precision[] NULL;

-- Thêm cột timeSpentHistory vào bảng UserFlashcardProgress
ALTER TABLE "UserFlashcardProgress"
ADD COLUMN IF NOT EXISTS "timeSpentHistory" double precision[] NULL;

ALTER TABLE public."UserFlashcardProgress"
ADD COLUMN IF NOT EXISTS "rating" int;

ALTER TABLE public."Container"
ADD COLUMN "cardsPerDay" integer DEFAULT 20;

-- Đầu tiên, cài đặt múi giờ cho phiên làm việc hiện tại
SET timezone = 'Asia/Ho_Chi_Minh';

-- Bảng Chapter
ALTER TABLE "Chapter" 
  ALTER COLUMN "createdAt" TYPE timestamp with time zone USING "createdAt" AT TIME ZONE 'Asia/Ho_Chi_Minh',
  ALTER COLUMN "updatedAt" TYPE timestamp with time zone USING "updatedAt" AT TIME ZONE 'Asia/Ho_Chi_Minh',
  ALTER COLUMN "deletedAt" TYPE timestamp with time zone USING "deletedAt" AT TIME ZONE 'Asia/Ho_Chi_Minh';

-- Bảng Curriculum
ALTER TABLE "Curriculum" 
  ALTER COLUMN "createdAt" TYPE timestamp with time zone USING "createdAt" AT TIME ZONE 'Asia/Ho_Chi_Minh',
  ALTER COLUMN "updatedAt" TYPE timestamp with time zone USING "updatedAt" AT TIME ZONE 'Asia/Ho_Chi_Minh',
  ALTER COLUMN "deletedAt" TYPE timestamp with time zone USING "deletedAt" AT TIME ZONE 'Asia/Ho_Chi_Minh';

-- Bảng Document
ALTER TABLE "Document" 
  ALTER COLUMN "createdAt" TYPE timestamp with time zone USING "createdAt" AT TIME ZONE 'Asia/Ho_Chi_Minh',
  ALTER COLUMN "updatedAt" TYPE timestamp with time zone USING "updatedAt" AT TIME ZONE 'Asia/Ho_Chi_Minh',
  ALTER COLUMN "deletedAt" TYPE timestamp with time zone USING "deletedAt" AT TIME ZONE 'Asia/Ho_Chi_Minh';

-- Bảng Enrollment
ALTER TABLE "Enrollment" 
  ALTER COLUMN "updatedAt" TYPE timestamp with time zone USING "updatedAt" AT TIME ZONE 'Asia/Ho_Chi_Minh',
  ALTER COLUMN "createdAt" TYPE timestamp with time zone USING "createdAt" AT TIME ZONE 'Asia/Ho_Chi_Minh';

-- Bảng EnrollmentProgress
ALTER TABLE "EnrollmentProgress" 
  ALTER COLUMN "updatedAt" TYPE timestamp with time zone USING "updatedAt" AT TIME ZONE 'Asia/Ho_Chi_Minh',
  ALTER COLUMN "createdAt" TYPE timestamp with time zone USING "createdAt" AT TIME ZONE 'Asia/Ho_Chi_Minh';

-- Bảng Exam
ALTER TABLE "Exam" 
  ALTER COLUMN "createdAt" TYPE timestamp with time zone USING "createdAt" AT TIME ZONE 'Asia/Ho_Chi_Minh',
  ALTER COLUMN "updatedAt" TYPE timestamp with time zone USING "updatedAt" AT TIME ZONE 'Asia/Ho_Chi_Minh',
  ALTER COLUMN "deletedAt" TYPE timestamp with time zone USING "deletedAt" AT TIME ZONE 'Asia/Ho_Chi_Minh';

-- Bảng ExamAnswer
ALTER TABLE "ExamAnswer" 
  ALTER COLUMN "createdAt" TYPE timestamp with time zone USING "createdAt" AT TIME ZONE 'Asia/Ho_Chi_Minh',
  ALTER COLUMN "updatedAt" TYPE timestamp with time zone USING "updatedAt" AT TIME ZONE 'Asia/Ho_Chi_Minh',
  ALTER COLUMN "deletedAt" TYPE timestamp with time zone USING "deletedAt" AT TIME ZONE 'Asia/Ho_Chi_Minh';

-- Bảng Flashcard
ALTER TABLE "Flashcard" 
  ALTER COLUMN "createdAt" TYPE timestamp with time zone USING "createdAt" AT TIME ZONE 'Asia/Ho_Chi_Minh',
  ALTER COLUMN "updatedAt" TYPE timestamp with time zone USING "updatedAt" AT TIME ZONE 'Asia/Ho_Chi_Minh',
  ALTER COLUMN "deletedAt" TYPE timestamp with time zone USING "deletedAt" AT TIME ZONE 'Asia/Ho_Chi_Minh';

-- Bảng FlashcardContent
ALTER TABLE "FlashcardContent" 
  ALTER COLUMN "createdAt" TYPE timestamp with time zone USING "createdAt" AT TIME ZONE 'Asia/Ho_Chi_Minh',
  ALTER COLUMN "updatedAt" TYPE timestamp with time zone USING "updatedAt" AT TIME ZONE 'Asia/Ho_Chi_Minh',
  ALTER COLUMN "deletedAt" TYPE timestamp with time zone USING "deletedAt" AT TIME ZONE 'Asia/Ho_Chi_Minh';

-- Bảng Lesson
ALTER TABLE "Lesson" 
  ALTER COLUMN "createdAt" TYPE timestamp with time zone USING "createdAt" AT TIME ZONE 'Asia/Ho_Chi_Minh',
  ALTER COLUMN "updatedAt" TYPE timestamp with time zone USING "updatedAt" AT TIME ZONE 'Asia/Ho_Chi_Minh',
  ALTER COLUMN "deletedAt" TYPE timestamp with time zone USING "deletedAt" AT TIME ZONE 'Asia/Ho_Chi_Minh';

-- Bảng Province
ALTER TABLE "Province" 
  ALTER COLUMN "createdAt" TYPE timestamp with time zone USING "createdAt" AT TIME ZONE 'Asia/Ho_Chi_Minh',
  ALTER COLUMN "updatedAt" TYPE timestamp with time zone USING "updatedAt" AT TIME ZONE 'Asia/Ho_Chi_Minh',
  ALTER COLUMN "deletedAt" TYPE timestamp with time zone USING "deletedAt" AT TIME ZONE 'Asia/Ho_Chi_Minh';

-- Bảng School
ALTER TABLE "School" 
  ALTER COLUMN "createdAt" TYPE timestamp with time zone USING "createdAt" AT TIME ZONE 'Asia/Ho_Chi_Minh',
  ALTER COLUMN "updatedAt" TYPE timestamp with time zone USING "updatedAt" AT TIME ZONE 'Asia/Ho_Chi_Minh',
  ALTER COLUMN "deletedAt" TYPE timestamp with time zone USING "deletedAt" AT TIME ZONE 'Asia/Ho_Chi_Minh';

-- Bảng Subject
ALTER TABLE "Subject" 
  ALTER COLUMN "createdAt" TYPE timestamp with time zone USING "createdAt" AT TIME ZONE 'Asia/Ho_Chi_Minh',
  ALTER COLUMN "updatedAt" TYPE timestamp with time zone USING "updatedAt" AT TIME ZONE 'Asia/Ho_Chi_Minh',
  ALTER COLUMN "deletedAt" TYPE timestamp with time zone USING "deletedAt" AT TIME ZONE 'Asia/Ho_Chi_Minh';

-- Bảng Theory
ALTER TABLE "Theory" 
  ALTER COLUMN "createdAt" TYPE timestamp with time zone USING "createdAt" AT TIME ZONE 'Asia/Ho_Chi_Minh',
  ALTER COLUMN "updatedAt" TYPE timestamp with time zone USING "updatedAt" AT TIME ZONE 'Asia/Ho_Chi_Minh',
  ALTER COLUMN "deletedAt" TYPE timestamp with time zone USING "deletedAt" AT TIME ZONE 'Asia/Ho_Chi_Minh';

-- Bảng UserLike
ALTER TABLE "UserLike" 
  ALTER COLUMN "createdAt" TYPE timestamp with time zone USING "createdAt" AT TIME ZONE 'Asia/Ho_Chi_Minh',
  ALTER COLUMN "updatedAt" TYPE timestamp with time zone USING "updatedAt" AT TIME ZONE 'Asia/Ho_Chi_Minh';

-- Bảng FolderUser
ALTER TABLE "FolderUser" 
  ALTER COLUMN "createdAt" TYPE timestamp with time zone USING "createdAt" AT TIME ZONE 'Asia/Ho_Chi_Minh';

-- Bảng FlashcardFolder
ALTER TABLE "FlashcardFolder" 
  ALTER COLUMN "createdAt" TYPE timestamp with time zone USING "createdAt" AT TIME ZONE 'Asia/Ho_Chi_Minh';

-- Bảng DocumentFolder
ALTER TABLE "DocumentFolder" 
  ALTER COLUMN "createdAt" TYPE timestamp with time zone USING "createdAt" AT TIME ZONE 'Asia/Ho_Chi_Minh';

-- Bảng Container
ALTER TABLE "Container" 
  ALTER COLUMN "viewAt" TYPE timestamp with time zone USING "viewAt" AT TIME ZONE 'Asia/Ho_Chi_Minh';

-- Bảng FSRSPreset
ALTER TABLE "FSRSPreset" 
  ALTER COLUMN "createdAt" TYPE timestamp with time zone USING "createdAt" AT TIME ZONE 'Asia/Ho_Chi_Minh',
  ALTER COLUMN "updatedAt" TYPE timestamp with time zone USING "updatedAt" AT TIME ZONE 'Asia/Ho_Chi_Minh';

-- Cập nhật giá trị mặc định cho các trường timestamp nếu cần
-- Ví dụ: 
ALTER TABLE "Chapter" 
  ALTER COLUMN "createdAt" SET DEFAULT CURRENT_TIMESTAMP;

-- Bảng Question
ALTER TABLE "Question" 
  ALTER COLUMN "createdAt" TYPE timestamp with time zone USING "createdAt" AT TIME ZONE 'Asia/Ho_Chi_Minh',
  ALTER COLUMN "updatedAt" TYPE timestamp with time zone USING "updatedAt" AT TIME ZONE 'Asia/Ho_Chi_Minh';

-- Bảng UserQuizProgress
ALTER TABLE "UserQuizProgress" 
  ALTER COLUMN "createdAt" TYPE timestamp with time zone USING "createdAt" AT TIME ZONE 'Asia/Ho_Chi_Minh',
  ALTER COLUMN "updatedAt" TYPE timestamp with time zone USING "updatedAt" AT TIME ZONE 'Asia/Ho_Chi_Minh';

-- Bảng QuestionAnswer
ALTER TABLE "QuestionAnswer" 
  ALTER COLUMN "createdAt" TYPE timestamp with time zone USING "createdAt" AT TIME ZONE 'Asia/Ho_Chi_Minh',
  ALTER COLUMN "updatedAt" TYPE timestamp with time zone USING "updatedAt" AT TIME ZONE 'Asia/Ho_Chi_Minh';

-- Kiểm tra xem có còn trường timestamp without timezone nào không
SELECT
    table_schema,
    table_name,
    column_name,
    data_type
FROM
    information_schema.columns
WHERE
    data_type = 'timestamp with time zone'
    AND table_schema = 'public';

ALTER TABLE public."Flashcard"
ADD COLUMN "isCreatedBySystem" boolean NOT NULL DEFAULT false;