CREATE TABLE "Role" (
                        "id" SERIAL PRIMARY KEY,
                        "roleName" VARCHAR(50) NOT NULL
);

CREATE TABLE "BaseUser" (
                            "id" UUID PRIMARY KEY,
                            "username" VARCHAR(50) NOT NULL,
                            "email" VARCHAR(100),
                            "bio" VARCHAR(255),
                            "fullname" VARCHAR(100),
                            "passwordSalt" VARCHAR(255),
                            "password" VARCHAR(255),
                            "roleId" SERIAL REFERENCES "Role"("id"),
                            "provider" VARCHAR(50),
                            "createdAt" TIMESTAMP NOT NULL DEFAULT CURRENT_TIMESTAMP,
                            "updatedAt" TIMESTAMP,
                            "deletedAt" TIMESTAMP,
                            "status" VARCHAR(20),
                            "refreshToken" VARCHAR(255),
                            "timezone" VARCHAR(50),
                            "lastLoginAt" TIMESTAMP,
                            "profilePicture" VARCHAR(255)
);

CREATE TABLE "ChosenSubjectCurriculum" (
    "id" UUID PRIMARY KEY,
    "userId" UUID,
    "subjectId" UUID,
    "curriculumId" UUID,
    "subjectCurriculumId" UUID,
    "createdAt" TIMESTAMP WITHOUT TIME ZONE DEFAULT CURRENT_TIMESTAMP,
    "updatedAt" TIMESTAMP WITHOUT TIME ZONE
);


CREATE TABLE "Student" (
                           "id" UUID PRIMARY KEY,
                           "baseUserId" UUID REFERENCES "BaseUser"("id") NOT NULL,
                           "grade" INT NOT NULL,
                           "schoolName" VARCHAR(100)
);

CREATE TABLE "Enrollment" (
                              "id" UUID PRIMARY KEY,
                              "studentId" UUID REFERENCES "Student"("id") NOT NULL,
                              "subjectId" INT NOT NULL,
                              "updatedAt" TIMESTAMP,
                              "createdAt" TIMESTAMP NOT NULL DEFAULT CURRENT_TIMESTAMP
);

CREATE TABLE "EnrollmentProcess" (
                                     "id" UUID PRIMARY KEY,
                                     "enrollmentId" UUID REFERENCES "Enrollment"("id") NOT NULL,
                                     "chapterId" INT NOT NULL,
                                     "lessonId" INT,
                                     "updatedAt" TIMESTAMP,
                                     "createdAt" TIMESTAMP NOT NULL DEFAULT CURRENT_TIMESTAMP,
                                     "deletedAt" TIMESTAMP
);

CREATE TABLE "RecentView" (
                              "id" UUID PRIMARY KEY,
                              "subjectId" UUID,
                              "discussionId" UUID,
                              "flashcardId" UUID,
                              "userId" UUID REFERENCES "BaseUser"("id") NOT NULL,
                              "updatedAt" TIMESTAMP,
                              "createdAt" TIMESTAMP NOT NULL DEFAULT CURRENT_TIMESTAMP,
                              "deletedAt" TIMESTAMP
);

CREATE TABLE "Teacher" (
                           "id" UUID PRIMARY KEY,
                           "baseUserId" UUID REFERENCES "BaseUser"("id") NOT NULL,
                           "graduatedUniversity" VARCHAR(100),
                           "contactNumber" VARCHAR(15),
                           "pin" VARCHAR(10),
                           "workPlace" VARCHAR(100),
                           "subjectsTaught" VARCHAR(255),
                           "rating" FLOAT CHECK ("rating" >= 0.0 AND "rating" <= 5.0),
                           "experienceYears" INT NOT NULL CHECK ("experienceYears" >= 0),
                           "verified" BOOL NOT NULL DEFAULT FALSE,
                           "videoIntroduction" VARCHAR(255)
);

CREATE TABLE "Certificate" (
                               "id" UUID PRIMARY KEY,
                               "teacherId" UUID REFERENCES "Teacher"("id") NOT NULL,
                               "certName" VARCHAR(100) NOT NULL,
                               "certLink" VARCHAR(255),
                               "issuedBy" VARCHAR(100),
                               "issueDate" TIMESTAMP,
                               "createdAt" TIMESTAMP NOT NULL DEFAULT CURRENT_TIMESTAMP,
                               "updatedAt" TIMESTAMP,
                               "deletedAt" TIMESTAMP
);

CREATE TABLE "Note" (
                        "id" UUID PRIMARY KEY,
                        "noteName" VARCHAR(100) NOT NULL,
                        "noteBody" TEXT,
                        "userId" UUID REFERENCES "BaseUser"("id") NOT NULL,
                        "createdAt" TIMESTAMP NOT NULL DEFAULT CURRENT_TIMESTAMP,
                        "updatedAt" TIMESTAMP,
                        "deletedAt" TIMESTAMP
);

CREATE TABLE "Report" (
                          "id" UUID PRIMARY KEY,
                          "reportTitle" VARCHAR(100) NOT NULL,
                          "reportContent" TEXT NOT NULL,
                          "status" VARCHAR(20) NOT NULL,
                          "userId" UUID REFERENCES "BaseUser"("id") NOT NULL
);

CREATE TABLE "OutboxMessage" (
    "eventId" UUID PRIMARY KEY,                  
    "eventPayload" TEXT NOT NULL,       
    "occurredOn" TIMESTAMP WITH TIME ZONE NOT NULL,
    "processedOn" TIMESTAMP WITH TIME ZONE,    
    "isMessageDispatched" BOOLEAN NOT NULL DEFAULT FALSE,
    "error" TEXT
);

create table "Session"
(
    id             uuid      not null
        constraint usersessions_pkey
            primary key,
    "userId"       uuid
        constraint "usersessions_UserId_fkey"
            references "BaseUser",
    "deviceInfo"   varchar,
    "refreshToken" text      not null,
    "expiredAt"    timestamp not null,
    "isRevoked"    boolean default false,
    "createdAt"    timestamp,
    "updatedAt"    timestamp,
    "ipAddress"    varchar
);
CREATE TABLE "ReportDocument" (
    "id" uuid NOT NULL,
    "reportTitle" varchar(255) NOT NULL,
    "reportContent" text NOT NULL,
    "status" varchar(20) NOT NULL,
    "reportType" text NOT NULL,
    "documentId" uuid NOT NULL,
    "userId" uuid NOT NULL,
    "createdAt" timestamp without time zone NOT NULL,
    CONSTRAINT "Report_document_pkey" PRIMARY KEY ("id"),
    CONSTRAINT "Report_document_userId_fkey" FOREIGN KEY ("userId")
        REFERENCES "BaseUser" ("id") ON DELETE SET NULL
);

