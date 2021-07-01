CREATE FUNCTION [dbo].[CanProcessJob]
(
	@collectionId int,
	@period int,
	@isFirstStage bit,
	@overrideFlag bit,
	@collectionTypeId int,
	@isMultiStage bit,
	@jobPriority int,
	@utcDateJobLastUpdated datetime
)
RETURNS int
AS
BEGIN
	--if override flag on then continue processing
	if @overrideFlag = 1  
		Return 1

	--if override flag 0 then do not process anything
	if @overrideFlag = 0
		Return 0
	
	--If reference data type or Period end or Generic collection then don't check return periods or multi-stage 
	if @collectionTypeId IN (4, 7, 8, 9, 10, 13, 14, 15)
		Return 1

	--If reference data type = collection type id check for any period end in progress. 
	if @collectionTypeId = 16 AND EXISTS(SELECT *
											FROM PeriodEnd.PeriodEnd P INNER JOIN RETURNPERIOD R on  P.periodid = R.returnperiodid
											INNER JOIN COLLECTION c on C.CollectionId = R.CollectionId
											WHERE CollectionTypeId = 1 and P.closed = 0)
		return 0

	--If ILR type then allow first stage 
	if @isMultiStage = 1 AND IsNull(@isFirstStage,1) = 1
		Return 1

	if @jobPriority = 99
		return 1

	--if period open and no override set its ok
	 If EXISTS (SELECT 1 FROm ReturnPeriod rp WHERE CollectionId = @collectionId AND PeriodNumber = @period
				AND (@utcDateJobLastUpdated BETWEEN rp.StartDateTimeUTC AND rp.EndDateTimeUTC OR GetUTCDate() BETWEEN rp.StartDateTimeUTC AND rp.EndDateTimeUTC))
		Return 1

	Return 0
END