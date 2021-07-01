
Update oc Set oc.Ukprn = o.Ukprn From 
OrganisationCollection oc
Inner Join Organisation o
On o.OrganisationId = oc.OrganisationId
Where oc.Ukprn is Null