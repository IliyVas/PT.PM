﻿using System;
using PT.PM.ParseTreeUst;
using PT.PM.UstParsing;
using PT.PM.Common;
using PT.PM.CSharpParseTreeUst;
using PT.PM.JavaParseTreeUst;
using PT.PM.SqlParseTreeUst;
using System.Collections.Generic;
using PT.PM.JavaScriptParseTreeUst;

namespace PT.PM
{
    public class ParserConverterBuilder
    {
        public static Dictionary<Language, ParserConverterSet> GetParserConverterSets(LanguageFlags languageFlags)
        {
            var languages = new List<Language>();
            languages.AddRange(languageFlags.GetLanguages());
            languages.AddRange(languageFlags.GetImpactLanguages());
            Dictionary<Language, ParserConverterSet> result = new Dictionary<Language, ParserConverterSet>();
            foreach (var language in languages)
            {
                result[language] = GetParserConverterSet(language);
                result[language].Converter.ConvertedLanguages &= languageFlags;
            }
            return result;
        }

        public static ParserConverterSet GetParserConverterSet(Language language)
        {
            var result = new ParserConverterSet();
            switch (language)
            {
                case Language.CSharp:
                    result.Parser = new CSharpRoslynParser();
                    result.Converter = new CSharpRoslynParseTreeConverter();
                    break;
                case Language.Java:
                    result.Parser = new JavaAntlrParser();
                    result.Converter = new JavaAntlrParseTreeConverter();
                    break;
                case Language.Php:
                    result.Parser = new PhpAntlrParser();
                    result.Converter = new PhpAntlrParseTreeConverter();
                    break;
                case Language.PlSql:
                    result.Parser = new PlSqlAntlrParser();
                    result.Converter = new PlSqlAntlrConverter();
                    break;
                case Language.TSql:
                    result.Parser = new TSqlAntlrParser();
                    result.Converter = new TSqlAntlrConverter();
                    break;
                case Language.Aspx:
                    result.Parser = new AspxPmParser();
                    result.Converter = new AspxConverter();
                    break;
                case Language.JavaScript:
                    result.Parser = new JavaScriptAntlrParser();
                    result.Converter = new JavaScriptParseTreeConverter();
                    break;
                case Language.Html:
                    result.Parser = new PhpAntlrParser();
                    result.Converter = new PhpAntlrParseTreeConverter();
                    break;
                default:
                    throw new NotImplementedException($"Language {language} is not supported");
            }
            return result;
        }
    }
}