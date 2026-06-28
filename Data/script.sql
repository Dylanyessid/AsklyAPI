
/*CREATE DATABASE Askly;

CREATE TABLE Users(
	Id INT PRIMARY KEY IDENTITY(1,1),
	Name VARCHAR(30) NOT NULL,
	LastName VARCHAR(30),
	Email VARCHAR(255) UNIQUE,
	HashedPassword VARCHAR(1024),
	CreatedAt DATETIME2(3) NOT NULL DEFAULT GETUTCDATE(),
	UpdatedAt DATETIME2(3) NOT NULL DEFAULT GETUTCDATE()
);

CREATE TABLE Tags(
    Id INT PRIMARY KEY IDENTITY(1,1),
    Name NVARCHAR(50) NOT NULL UNIQUE,
    CreatedAt DATETIME2(3) NOT NULL DEFAULT GETUTCDATE(),
	UpdatedAt DATETIME2(3) NOT NULL DEFAULT GETUTCDATE() 
);

INSERT INTO Tags (Name) VALUES ('Matemáticas');
INSERT INTO Tags (Name) VALUES ('Química');
INSERT INTO Tags (Name) VALUES ('Física');
INSERT INTO Tags (Name) VALUES ('Historia');
INSERT INTO Tags (Name) VALUES ('Programación');
INSERT INTO Tags (Name) VALUES ('Lenguas');

CREATE TABLE Questions(
	Id INT PRIMARY KEY IDENTITY(1,1),
	UserId INT NOT NULL FOREIGN KEY REFERENCES Users(Id),
    TagId INT NOT NULL FOREIGN KEY REFERENCES Tags(Id),
	Title NVARCHAR(100) NOT NULL,
	Body NVARCHAR(1500),
	IsSolved BIT NOT NULL,
	CreatedAt DATETIME2(3) NOT NULL DEFAULT GETUTCDATE(),
	UpdatedAt DATETIME2(3) NOT NULL DEFAULT GETUTCDATE(),
    DeletedAt DATETIME2(3) NULL
);



CREATE TABLE Answers (
    Id INT IDENTITY(1,1) PRIMARY KEY,
    QuestionId INT NOT NULL FOREIGN KEY REFERENCES Questions(Id),
    UserId INT NOT NULL FOREIGN KEY REFERENCES Users(Id),
    Body NVARCHAR(MAX) NOT NULL,
    IsAccepted BIT NOT NULL DEFAULT 0,
    VoteCount INT NOT NULL DEFAULT 0,
    CreatedAt DATETIME2 NOT NULL DEFAULT SYSUTCDATETIME(),
    UpdatedAt DATETIME2 NOT NULL DEFAULT SYSUTCDATETIME(),
    DeletedAt DATETIME2 NULL
);

CREATE TABLE AnswerVotes (
    Id INT IDENTITY(1,1) PRIMARY KEY,
    AnswerId INT NOT NULL FOREIGN KEY REFERENCES Answers(Id),
    UserId INT NOT NULL FOREIGN KEY REFERENCES Users(Id),
    VoteType TINYINT NOT NULL,
    CreatedAt DATETIME2 NOT NULL DEFAULT SYSUTCDATETIME(),
    UpdatedAt DATETIME2 NOT NULL DEFAULT SYSUTCDATETIME(),
    CONSTRAINT UQ_AnswerVotes_AnswerId_UserId
        UNIQUE (AnswerId, UserId)

);*/



CREATE TABLE users (
    id SERIAL PRIMARY KEY,
    name VARCHAR(30) NOT NULL,
    last_name VARCHAR(30),
    email VARCHAR(255) UNIQUE,
    hashed_password VARCHAR(1024),
    created_at TIMESTAMPTZ NOT NULL DEFAULT NOW(),
    updated_at TIMESTAMPTZ NOT NULL DEFAULT NOW()
);

CREATE TABLE tags (
    id SERIAL PRIMARY KEY,
    name VARCHAR(50) NOT NULL UNIQUE,
    created_at TIMESTAMPTZ NOT NULL DEFAULT NOW(),
    updated_at TIMESTAMPTZ NOT NULL DEFAULT NOW()
);

INSERT INTO tags (name) VALUES 
    ('Matemáticas'),
    ('Química'),
    ('Física'),
    ('Historia'),
    ('Programación'),
    ('Lenguas');

CREATE TABLE questions (
    id SERIAL PRIMARY KEY,
    user_id INT NOT NULL REFERENCES users(id),
    tag_id INT NOT NULL REFERENCES tags(id),
    title VARCHAR(100) NOT NULL,
    body VARCHAR(1500),
    is_solved BOOLEAN NOT NULL DEFAULT FALSE,
    language TEXT NOT NULL DEFAULT 'spanish',
    search_vector TSVECTOR GENERATED ALWAYS AS (
        to_tsvector(language::regconfig, coalesce(title, '') || ' ' || coalesce(body, ''))
    ) STORED,
    created_at TIMESTAMPTZ NOT NULL DEFAULT NOW(),
    updated_at TIMESTAMPTZ NOT NULL DEFAULT NOW(),
    deleted_at TIMESTAMPTZ NULL
);

CREATE INDEX idx_questions_search ON questions USING GIN(search_vector);

CREATE TABLE answers (
    id SERIAL PRIMARY KEY,
    question_id INT NOT NULL REFERENCES questions(id),
    user_id INT NOT NULL REFERENCES users(id),
    body TEXT NOT NULL,
    is_accepted BOOLEAN NOT NULL DEFAULT FALSE,
    vote_count INT NOT NULL DEFAULT 0,
    created_at TIMESTAMPTZ NOT NULL DEFAULT NOW(),
    updated_at TIMESTAMPTZ NOT NULL DEFAULT NOW(),
    deleted_at TIMESTAMPTZ NULL
);

CREATE TABLE answer_votes (
    id SERIAL PRIMARY KEY,
    answer_id INT NOT NULL REFERENCES answers(id),
    user_id INT NOT NULL REFERENCES users(id),
    vote_type SMALLINT NOT NULL,
    created_at TIMESTAMPTZ NOT NULL DEFAULT NOW(),
    updated_at TIMESTAMPTZ NOT NULL DEFAULT NOW(),
    CONSTRAINT uq_answer_votes_answer_user UNIQUE (answer_id, user_id)
);