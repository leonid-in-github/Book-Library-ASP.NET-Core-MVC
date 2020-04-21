
CREATE PROCEDURE [dbo].[GetSessionLastRenewalDate]
	@SessionId CHAR(24),
	@SessionLastRenewalDate DATETIME OUTPUT
AS
BEGIN
	DECLARE @SessionCloseDate DATETIME;
	SELECT @SessionCloseDate = CloseDate
	FROM Sessions
	WHERE SessionId = @SessionId;

	IF @SessionCloseDate IS NULL
	BEGIN
		SELECT @SessionLastRenewalDate = LastRenewalDate
		FROM Sessions
		WHERE SessionId = @SessionId;
		IF @SessionLastRenewalDate IS NULL
		BEGIN
			SELECT @SessionLastRenewalDate = OpenDate
			FROM Sessions
			WHERE SessionId = @SessionId;
		END
	END
	ELSE
	BEGIN
		SET @SessionLastRenewalDate = CAST('1753-1-1' AS DATETIME);
	END
END;
