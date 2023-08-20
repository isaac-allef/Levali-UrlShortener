-- USE [master]

CREATE TABLE dbo.Users(
	Id int IDENTITY NOT NULL,
	Name VARCHAR(255) NOT NULL,
	Email VARCHAR(255) NOT NULL UNIQUE,
	PasswordHash VARCHAR(255) NOT NULL,
    CreatedAt DATETIME NOT NULL,
    -- UpdatedAt DATETIME NOT NULL,
	CONSTRAINT PK_user_id PRIMARY KEY (Id)
)

CREATE TABLE dbo.ShortUrls(
	Code VARCHAR(6) NOT NULL,
	TargetUrl VARCHAR(MAX) NOT NULL,
    ExpirationAt DATETIME,
    CreatedAt DATETIME NOT NULL,
	UserId int NOT NULL,
	CONSTRAINT PK_short_url_code PRIMARY KEY (Code),
	CONSTRAINT FK_short_url_user_id FOREIGN KEY (UserId) REFERENCES dbo.Users(Id)
)

CREATE TABLE dbo.ShortUrlsAnalytics(
	Id int IDENTITY NOT NULL,
	ClickCount int NOT NULL,
	LastClick DATETIME,
	ShortUrlCode VARCHAR(6) NOT NULL,
	CONSTRAINT PK_short_urls_analytics_id PRIMARY KEY (Id),
	CONSTRAINT FK_short_url_code FOREIGN KEY (ShortUrlCode) REFERENCES dbo.ShortUrls(Code) ON DELETE CASCADE
)
