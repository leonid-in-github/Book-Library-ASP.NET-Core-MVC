
CREATE PROCEDURE [dbo].[GetSessionUser]
	@SessionId CHAR(36),
	@AccountId INT OUTPUT,
	@Login NVARCHAR(32) OUTPUT,
	@FirstName NVARCHAR(32) OUTPUT,
	@LastName NVARCHAR(32) OUTPUT,
	@Email NVARCHAR(32) OUTPUT
AS
BEGIN
	SELECT @AccountId = AccountsV.ID,
			@Login = AccountsV.Login,
			@FirstName = ProfilesV.FirstName,
			@LastName = ProfilesV.LastName,
			@Email = ProfilesV.Email
	FROM 
	(SELECT AccountId FROM Sessions WHERE SessionId = @SessionId) AS SessionV
	INNER JOIN 
	Accounts AS AccountsV ON SessionV.AccountId = AccountsV.ID
	INNER JOIN
	Profiles AS ProfilesV ON AccountsV.ProfileId = ProfilesV.ID;
END;

