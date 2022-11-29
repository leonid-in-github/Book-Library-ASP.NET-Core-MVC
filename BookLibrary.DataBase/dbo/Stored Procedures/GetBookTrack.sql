


CREATE PROCEDURE [dbo].[GetBookTrack]
	@BookID INT,
	@TracksCount NVARCHAR(10)
AS
BEGIN
	IF (@TracksCount = 'All')
	BEGIN
		SELECT BookId, Name AS BookName, Login, ProfileV.Email as Email, ActionTime, Action
		FROM 
			(SELECT * from dbo.BookTracking where BookId = @BookID) as TrackV
			INNER JOIN
			dbo.Books as BookV ON  BookV.ID = TrackV.BookId
			INNER JOIN
			dbo.Accounts AS AccountV ON AccountV.ID = TrackV.AccountId
			INNER JOIN
			dbo.Profiles AS ProfileV ON AccountV.ProfileId = ProfileV.ID
		ORDER BY ActionTime DESC;
	END;
	ELSE
	BEGIN
		DECLARE @TCount INT;
		
		SET @TCount = CASE WHEN TRY_CAST(@TracksCount AS INT) IS NULL
						THEN 10
						ELSE CAST(@TracksCount AS INT)
						END;
		SELECT TOP(@TCount) BookId, Name AS BookName, Login, ProfileV.Email as Email, ActionTime, Action
		FROM 
			(SELECT * from dbo.BookTracking where BookId = @BookID) as TrackV
			INNER JOIN
			dbo.Books as BookV ON  BookV.ID = TrackV.BookId
			INNER JOIN
			dbo.Accounts AS AccountV ON AccountV.ID = TrackV.AccountId
			INNER JOIN
			dbo.Profiles AS ProfileV ON AccountV.ProfileId = ProfileV.ID
		ORDER BY ActionTime DESC;
	END;
END;