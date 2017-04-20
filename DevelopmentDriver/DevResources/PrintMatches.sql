SELECT CONCAT (v.Infinative, '( ', 
			  v.EnglishInfinative, ')', ' applies rule ', 
			  CR.Name, ' for person ', 
			  P.SpanishExpression, ' with string ', CM.ConjugationString)
FROM Verbs v
JOIN ConjugationMatches CM ON v.Id = CM.VerbId
JOIN ConjugationRules CR on CM.ConjugationRuleId = CR.Id
JOIN Persons P on p.Id = CM.PersonId