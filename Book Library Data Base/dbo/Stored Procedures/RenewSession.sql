
CREATE PROCEDURE [dbo].[RenewSession]
	@SessionId CHAR(36),
	@RenewDate DATETIME,
	@Result INT OUTPUT
AS
	IF EXISTS (SELECT * FROM Sessions 
                   WHERE SessionId = @SessionId
				   )
	BEGIN
		UPDATE Sessions 
		SET LastRenewalDate = @RenewDate
		WHERE SessionId = @SessionId
		SET @Result = 1;
	END
	ELSE
		SET @Result = 0;

