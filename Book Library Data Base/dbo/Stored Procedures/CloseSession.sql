
CREATE PROCEDURE [dbo].[CloseSession]
	@SessionId CHAR(36),
	@CloseDate DATETIME,
	@Result INT OUTPUT
AS
	IF EXISTS (SELECT * FROM Sessions 
                   WHERE SessionId = @SessionId
				   )
	BEGIN
		UPDATE Sessions 
		SET CloseDate = @CloseDate
		WHERE SessionId = @SessionId
		SET @Result = 1;
	END
	ELSE
		SET @Result = 0;

