using NotaDAL;
using NotaDAL.Models;
using NotadoraModels;
using NotadoraModels.Models;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NotadoraDataMigration
{
    class Program
    {
        static void Main(string[] args)
        {
            var context = new NotaContext();
            using (var oldContext = new NotadoraContext())
            {
                var _verbs = oldContext.Verbs;
                var _conjugators = oldContext.Conjugators;
                var _verbsConjugators = oldContext.VerbsConjugators;
                var _grammPersons = oldContext.GrammaticalPersons;
                var _tenses = oldContext.Tenses;
                var _instructions = oldContext.ConjugationInstructions;

                //foreach (var _person in _grammPersons)
                //{
                //    var p = new Person
                //    {
                //        SpanishExpression = "",
                //        Description = _person.Name,
                //        Formality = PersonFormality.Informal,
                //        Gender = PersonGender.None,
                //        Order = PersonOrder.None,
                //        Plurality = PersonPlurality.Single,
                //    };

                //    context.Persons.InsertOnSubmit(p);
                //    context.SubmitChanges();                      
                //}

                oldContext.Verbs.Include(v => v.VerbConjugators.Select(vc => vc.Conjugator)).Load();
                _instructions.Include(i => i.GrammaticalPerson).Load();
                _instructions.Load();
                _conjugators.Include(c => c.Tense).Load();
                _conjugators.Include(c => c.ConjugatorGrammaticalPersons).Load();

                //foreach (var _tense in _tenses)
                //{
                //    var tense = new NotaDAL.Models.Tense
                //    {
                //        Name = _tense.Name,
                //        Description = "Needs Description",
                //        RugularConjugationRuleId = -1
                //    };

                //    context.Tenses.InsertOnSubmit(tense);
                //    context.SubmitChanges();
                //}

                foreach (var _instruction in _instructions)
                {
                    var suffix = (!string.IsNullOrEmpty(_instruction.AppendedSuffix))
                                    ? _instruction.AppendedSuffix : null;

                    var _conjugator = _instruction.Conjugator;
                    var _conjugatorVerbs = _conjugator.ConjugatorVerbs;
                    var conjugationType = translateConjugatorType(_conjugator);
                    var tense = context.Tenses.First(t => t.Name == _conjugator.Tense.Name);

                    var conjugationRule = context.ConjugationRules.FirstOrDefault(cr => cr.Name == _conjugator.Name)
                        ?? new ConjugationRule
                        {
                            Description = "Needs Description",
                            Name = _conjugator.Name,
                            TenseId = tense.Id,
                            Type = conjugationType,
                            IsRegular = _conjugator.isRegular,
                            PersonCount = _conjugator.ConjugatorGrammaticalPersons.Count
                        };

                    if (conjugationRule.Id <= 0)
                    {
                        context.ConjugationRules.InsertOnSubmit(conjugationRule);
                        context.SubmitChanges();
                    }

                    var _conjugatorGrammPersonNames = _conjugator.ConjugatorGrammaticalPersons.Select(cgp => cgp.GrammaticalPerson.Name);

                    foreach (var _conjugatorGrammPersonName in _conjugatorGrammPersonNames)
                    {
                        var person = context.Persons.FirstOrDefault(p => p.Description.StartsWith(_conjugatorGrammPersonName));
                        var conjugationRulePerson = context.ConjugationRulePersons.FirstOrDefault(crp => crp.ConjugationRuleId == conjugationRule.Id &&
                                                                                                         crp.PersonId == person.Id)
                            ?? new ConjugationRulePerson
                            {
                                ConjugationRuleId = conjugationRule.Id,
                                PersonId = person.Id
                            };

                        if (conjugationRulePerson.Id <= 0)
                        { 
                            context.ConjugationRulePersons.InsertOnSubmit(conjugationRulePerson);
                            context.SubmitChanges();
                        }
                    }

                    if (_conjugator.isRegular && tense.RugularConjugationRuleId <= 0)
                    {
                        tense.RugularConjugationRuleId = conjugationRule.Id;
                    }

                    foreach (var _conjugatorVerb in _conjugatorVerbs)
                    {
                        var _verb = _conjugatorVerb.Verb;

                        var verb = context.Verbs.FirstOrDefault(v => v.Infinative == _verb.Infinative)
                        ?? new NotaDAL.Models.Verb
                        {
                            Description = "Needs Description",
                            EnglishInfinative = _verb.EnglishInfinative,
                            Infinative = _verb.Infinative,
                        };

                        if (verb.Id <= 0)
                        {
                            context.Verbs.InsertOnSubmit(verb);
                            context.SubmitChanges();
                        }

                        var verbConjugationRule = context.VerbsConjugationRules.FirstOrDefault(vcr => vcr.ConjugationRuleId == conjugationRule.Id && vcr.VerbId == verb.Id)
                        ?? new VerbsConjugationRule
                        {
                            ConjugationRuleId = conjugationRule.Id,
                            VerbId = verb.Id,
                            ConjugationData = (!string.IsNullOrEmpty(_conjugatorVerb.ConjData))
                                                ? _conjugatorVerb.ConjData : null
                        };

                        if (verbConjugationRule.Id <= 0)
                        {
                            context.VerbsConjugationRules.InsertOnSubmit(verbConjugationRule);
                            context.SubmitChanges();
                        }

                        var person = context.Persons.First(p => p.Description.StartsWith(_instruction.GrammaticalPerson.Name));

                        var verbType = getVerbType(_instruction);

                        var conjugationRulesInstruction = context.VerbConjugationInstructions.
                                                                  FirstOrDefault(vci => vci.ConjugationRuleId == conjugationRule.Id &&
                                                                                        vci.PersonId == person.Id &&
                                                                                        vci.VerbType == verbType)
                        ?? new ConjugationRulesInstruction
                        {
                            ConjugationRuleId = conjugationRule.Id,
                            PersonId = person.Id,
                            VerbType = verbType,
                            Suffix = _instruction.AppendedSuffix
                        };
                        

                        if (conjugationRulesInstruction.Id <= 0)
                        {
                            context.VerbConjugationInstructions.InsertOnSubmit(conjugationRulesInstruction);
                            context.SubmitChanges();
                        }
                    }
                }
            }

            return;
        }

        private static NotaDAL.Models.VerbType getVerbType(ConjugationInstruction _conjugatorInstruciton)
        {
            switch (_conjugatorInstruciton.VerbType)
            {
                case NotadoraModels.Models.VerbType.ar:
                    return NotaDAL.Models.VerbType.ar;
                case NotadoraModels.Models.VerbType.er:
                    return NotaDAL.Models.VerbType.er;
                case NotadoraModels.Models.VerbType.ir:
                    return NotaDAL.Models.VerbType.ir;
                default:
                    return NotaDAL.Models.VerbType.ar;
            }
        }
        private static NotaDAL.Models.ConjugationType translateConjugatorType(Conjugator _conjugator)
        {
            switch (_conjugator.Type)
            {
                case ConjugatorType.Independent:
                    return ConjugationType.Independent;
                case ConjugatorType.StemChange:
                    return ConjugationType.NewStemDependent;
                case ConjugatorType.InfChange:
                    return ConjugationType.NewInfDependent;
                case ConjugatorType.Special:
                    return ConjugationType.SpecialConjugation;
                default:
                    return ConjugationType.Independent;
            }
        }

    }
}
