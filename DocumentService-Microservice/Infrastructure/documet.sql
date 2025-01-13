

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
    "createdAt" timestamp without time zone DEFAULT CURRENT_TIMESTAMP NOT NULL,
    "updatedAt" timestamp without time zone,
    "deletedAt" timestamp without time zone,
    PRIMARY KEY(id)
    );

CREATE TABLE "Curriculum" (
                              id UUID PRIMARY KEY not null,
                              "curriculumName" VARCHAR(1000),
                              "createdAt" timestamp without time zone DEFAULT CURRENT_TIMESTAMP NOT NULL,
                              "updatedAt" timestamp without time zone,
                              "deletedAt" timestamp without time zone
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
    "createdAt" timestamp without time zone DEFAULT CURRENT_TIMESTAMP NOT NULL,
    "updatedAt" timestamp without time zone,
    "createdBy" uuid,
    "updatedBy" uuid,
    "deletedAt" timestamp without time zone,
    "schoolId" uuid,
    "subjectCurriculumId" uuid,
    "semester" integer,
    PRIMARY KEY(id)
    );


CREATE TABLE IF NOT EXISTS "Enrollment" (
                                            id uuid NOT NULL,
                                            "baseUserId" uuid NOT NULL,
                                            "subjectCurriculumId" uuid NOT NULL,
                                            "updatedAt" timestamp without time zone,
                                            "createdAt" timestamp without time zone DEFAULT CURRENT_TIMESTAMP NOT NULL,
                                            PRIMARY KEY(id)
    );


CREATE TABLE IF NOT EXISTS "EnrollmentProgress" (
                                                    id uuid NOT NULL,
                                                    "enrollmentId" uuid NOT NULL,
                                                    "lessonId" uuid NOT NULL,
                                                    "updatedAt" timestamp without time zone,
                                                    "createdAt" timestamp without time zone DEFAULT CURRENT_TIMESTAMP NOT NULL,
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
    "createdAt" timestamp without time zone DEFAULT CURRENT_TIMESTAMP NOT NULL,
    "updatedAt" timestamp without time zone,
    "deletedAt" timestamp without time zone,
    PRIMARY KEY(id)
    );


CREATE TABLE IF NOT EXISTS "ExamAnswer" (
                                            id uuid NOT NULL,
                                            "examId" uuid NOT NULL,
                                            "createdAt" timestamp without time zone DEFAULT CURRENT_TIMESTAMP NOT NULL,
                                            "updatedAt" timestamp without time zone,
                                            "createdBy" character varying(50),
    "updatedBy" character varying(50),
    "deletedAt" timestamp without time zone,
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
    "totalView" integer,
    "todayView" integer
    "createdAt" timestamp without time zone DEFAULT CURRENT_TIMESTAMP NOT NULL,
    "updatedAt" timestamp without time zone,
    "createdBy" character varying(50),
    "updatedBy" character varying(50),
    "deletedAt" timestamp without time zone,
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
    "createdAt" timestamp without time zone DEFAULT CURRENT_TIMESTAMP NOT NULL,
    "updatedAt" timestamp without time zone,
    "createdBy" character varying(50),
    "updatedBy" character varying(50),
    "deletedAt" timestamp without time zone,
    "flashcardContentTermRichText" character varying,
    rank integer,
    "flashcardContentDefinitionRichText" character varying,
    PRIMARY KEY(id)
    );


CREATE TABLE IF NOT EXISTS "FlashcardTag" (
                                              id uuid NOT NULL,
                                              "flashcardId" bigint NOT NULL,
                                              "tagId" bigint NOT NULL,
                                              "deletedAt" timestamp without time zone,
                                              PRIMARY KEY(id)
    );


CREATE TABLE IF NOT EXISTS "FlashcardTheory" (
                                                 id uuid NOT NULL,
                                                 term character varying(500) NOT NULL,
    definition character varying(500),
    "termRichText" character varying(500),
    "definitionRichText" character varying(500),
    "theoryId" uuid NOT NULL,
    "createdAt" timestamp without time zone DEFAULT CURRENT_TIMESTAMP NOT NULL,
    "updatedAt" timestamp without time zone,
    "deletedAt" timestamp without time zone,
    PRIMARY KEY(id)
    );


CREATE TABLE IF NOT EXISTS "Lesson" (
                                        id uuid NOT NULL,
                                        "chapterId" uuid NOT NULL,
                                        "lessonName" character varying(1000) NOT NULL,
    "lessonBody" character varying,
    "lessonMaterial" character varying,
    slug character varying(1000) NOT NULL,
    "like" integer,
    "displayOrder" integer,
    "createdAt" timestamp without time zone DEFAULT CURRENT_TIMESTAMP NOT NULL,
    "updatedAt" timestamp without time zone,
    "deletedAt" timestamp without time zone,
    "videoUrl" character varying(1000),
    PRIMARY KEY(id)
    );


CREATE TABLE IF NOT EXISTS "Province" (
                                          id integer NOT NULL,
                                          "provinceName" character varying(1000),
    "createdAt" timestamp without time zone,
    "updatedAt" timestamp without time zone,
    "deletedAt" timestamp without time zone,
    PRIMARY KEY(id)
    );

CREATE TABLE IF NOT EXISTS "RecommendedData" (
                                                 id uuid NOT NULL,
                                                 "userId" uuid NOT NULL,
                                                 "subjectIds" character varying NOT NULL,
                                                 "flashcardIds" character varying,
                                                 "documentIds" character varying,
                                                 "typeExam" character varying,
                                                 grade character varying,
                                                 "objectId" character varying,
                                                 PRIMARY KEY(id)
    );


CREATE TABLE IF NOT EXISTS "School" (
                                        id uuid NOT NULL,
                                        "schoolName" character varying(1000),
    "provinceId" integer,
    "locationDetail" character varying(1000),
    "createdAt" timestamp without time zone,
    "updatedAt" timestamp without time zone,
    "deletedAt" timestamp without time zone,
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
    "createdAt" timestamp without time zone DEFAULT CURRENT_TIMESTAMP NOT NULL,
    "updatedAt" timestamp without time zone,
    "deletedAt" timestamp without time zone,
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
    "createdAt" timestamp without time zone DEFAULT CURRENT_TIMESTAMP NOT NULL,
    "updatedAt" timestamp without time zone,
    "deletedAt" timestamp without time zone,
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
    "flashcardId" UUID NOT NULL,
    "correctCount" INT DEFAULT 0,
    "lastStudiedAt" TIMESTAMP WITH TIME ZONE,
    "isMastered" BOOLEAN DEFAULT FALSE,
    "easeFactor" FLOAT DEFAULT 2.5,       -- E-Factor for spaced repetition, starting with a default of 2.5
    "interval" INT DEFAULT 1,             -- Interval for review sessions, defaulting to 1 session
    "repetitionCount" INT DEFAULT 0,      -- Count of consecutive correct repetitions
    CONSTRAINT "fk_flashcardContent" FOREIGN KEY ("flashcardContentId") REFERENCES "FlashcardContent" ("id") ON DELETE CASCADE,
    CONSTRAINT "fk_flashcard" FOREIGN KEY ("flashcardId") REFERENCES "Flashcard" ("id") ON DELETE CASCADE
);

CREATE TABLE "FlashcardStudySession" (
    "id" UUID PRIMARY KEY,
    "userId" UUID NOT NULL,
    "flashcardId" UUID NOT NULL,
    "startTime" TIMESTAMP WITH TIME ZONE NOT NULL DEFAULT now(),
    "endTime" TIMESTAMP WITH TIME ZONE,
    "questionsAttempted" INT DEFAULT 0,
    "correctAnswers" INT DEFAULT 0,
    CONSTRAINT "fk_flashcard" FOREIGN KEY ("flashcardId") REFERENCES "Flashcard" ("id") ON DELETE CASCADE
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