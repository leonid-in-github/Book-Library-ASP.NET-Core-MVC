
CREATE PROCEDURE [dbo].[OpenSession]
	@AccountId INT,
	@OpenDate DATETIME,
	@SessionId CHAR(36),
	@Result INT OUTPUT
AS
	IF NOT EXISTS (SELECT * FROM Sessions 
                   WHERE SessionId = @SessionId
				   )
	BEGIN
		INSERT INTO Sessions (AccountId, OpenDate, SessionId)
		VALUES (@AccountId, @OpenDate, @SessionId);
		SET @Result = 1;
	END
	ELSE 
		SET @Result = 0;
