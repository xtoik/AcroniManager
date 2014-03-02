/** Copyright 2014 Álvaro Rodríguez Otero and Álvaro Rodrigo Yuste 
*
* Licensed under the EUPL, Version 1.1 or – as soon they will be
* approved by the European Commission – subsequent versions of the
* EUPL (the "Licence");* you may not use this work except in
* compliance with the Licence.
* You may obtain a copy of the Licence at:
*
* http://www.osor.eu/eupl/european-union-public-licence-eupl-v.1.1
*
* Unless required by applicable law or agreed to in writing,
* software distributed under the Licence is distributed on an "AS
* IS" BASIS, * WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either
* express or implied.
* See the Licence for the specific language governing permissions
* and limitations under the Licence.
*/

using AcroniManager.Core.Information;
using AcroniManager.Core.Matcher;
using AcroniManager.Patterns.FormatString;
using AcroniManager.Patterns.UnitTests.TestAdapter;
using NUnit.Framework;
using System.Collections.Generic;
using System.Collections.Specialized;

namespace AcroniManager.Patterns.UnitTests.FormatString
{
    class FormatStringPatternTests
    {
        #region Common Methods

        private void ExecuteTest(string patternConfiguration, string input, List<FoundMatchBase> expectedOutput, 
                                 bool checkDefinition = false, bool cutBeginOfDefinition = false, bool cutEndOfDefinition = false)
        {
            NameValueCollection config = new NameValueCollection();
            config.Add("formatString", patternConfiguration);
            if (checkDefinition)
            {
                config.Add("checkDefinition", "true");
            }
            if (cutBeginOfDefinition)
            {
                config.Add("cutBeginOfDefinition", "true");
                config.Add("beginOfDefinitionBoundaries", "unos,unas,una,un");
            }
            if (cutEndOfDefinition)
            {
                config.Add("cutEndOfDefinition", "true");
            }
            ResourceInformation inputResource = new ResourceInformation() { Content = input };            
            PatternTestAdapter<FormatStringPattern> ret = new PatternTestAdapter<FormatStringPattern>(config, inputResource, expectedOutput); 
            ret.ExecuteTest();
        }        

        #endregion Common Methods

        #region Disambiguation page

        private const string _disambiguationFormatString = @"{acronymRE} puede referirse a:(?:\s*\*\s*(?<definition>[^\r\n]*))+";

        [Test]
        public void DisambiguationNoResult()
        {
            ExecuteTest(_disambiguationFormatString, "AAA\r\nAAA puede referirse.", new List<FoundMatchBase>());
        }

        [Test]
        public void DisambiguationSingleResult()
        {
            ExecuteTest(_disambiguationFormatString,
                        "AAA\r\nAAA puede referirse a:\r* La pila AAA, pila seca de uso común en dispositivos electrónicos portátiles.\n",
                        new List<FoundMatchBase> 
                            { 
                                new FoundMatchBase 
                                    { 
                                        Match = "AAA",  
                                        Definition = "La pila AAA, pila seca de uso común en dispositivos electrónicos portátiles."
                                    } 
                            });
        }

        [Test]
        public void DisambiguationMultipleDefinitions()
        {
            ExecuteTest(_disambiguationFormatString,
                        "AAA\r\nAAA puede referirse a:\r* La pila AAA, pila seca de uso común en dispositivos electrónicos portátiles.\n"
                        + "* Lucha Libre AAA: Héroes del Ring, videojuego de lucha libre mexicana.\r",
                        new List<FoundMatchBase> 
                            { 
                                new FoundMatchBase 
                                    { 
                                        Match = "AAA",  
                                        Definition = "La pila AAA, pila seca de uso común en dispositivos electrónicos portátiles."
                                    },
                                new FoundMatchBase 
                                    { 
                                        Match = "AAA",  
                                        Definition = "Lucha Libre AAA: Héroes del Ring, videojuego de lucha libre mexicana."
                                    }
                            });
        }

        [Test]
        public void DisambiguationMultipleResults()
        {
            ExecuteTest(_disambiguationFormatString,
                        "AAA\r\nAAA puede referirse a:\r* La pila AAA, pila seca de uso común en dispositivos electrónicos portátiles.\n"
                        + "* Lucha Libre AAA: Héroes del Ring, videojuego de lucha libre mexicana.\r"
                        + "AAC puede referirse a:\r\n* La AAC (Asociación Argentina de Cirugía).\n"
                        + "* AAC, código IATA del Aeropuerto Internacional de El Arish (Egipto).\n\r",
                        new List<FoundMatchBase> 
                            { 
                                new FoundMatchBase 
                                    { 
                                        Match = "AAA",  
                                        Definition = "La pila AAA, pila seca de uso común en dispositivos electrónicos portátiles."
                                    },
                                new FoundMatchBase 
                                    { 
                                        Match = "AAA",  
                                        Definition = "Lucha Libre AAA: Héroes del Ring, videojuego de lucha libre mexicana."
                                    },
                                new FoundMatchBase 
                                    { 
                                        Match = "AAC",  
                                        Definition = "La AAC (Asociación Argentina de Cirugía)."
                                    },
                                new FoundMatchBase 
                                    { 
                                        Match = "AAC",  
                                        Definition = "AAC, código IATA del Aeropuerto Internacional de El Arish (Egipto)."
                                    }
                            });
        }

        #endregion Disambiguation page

        #region Definition enclosed in parentheses

        private const string _defInParenthesesFormatString = @"{acronymRE}\s*\((?<definition>[^)]*)\)";

        [Test]
        public void DefinitionEnclosedInParenthesesNoResult()
        {
            ExecuteTest(_defInParenthesesFormatString, "AAA\r\nAAA puede referirse.", new List<FoundMatchBase>(), true);
        }

        [Test]
        public void DefinitionEnclosedInParenthesesResultWithInvalidDefinition()
        {
            ExecuteTest(_defInParenthesesFormatString, 
                        "\n* Sitio web de BMW (plurilingüe)\r* Grupo BMW (en inglés y alemán)\r\n", 
                        new List<FoundMatchBase>(), 
                        true);
        }

        [Test]
        public void DefinitionEnclosedInParenthesesMultipleResultsWithNotCheckedDefinitions()
        {
            ExecuteTest(_defInParenthesesFormatString,
                        "\n* Sitio web de BMW (plurilingüe)\r* Grupo BMW (en inglés y alemán)\r\n",
                        new List<FoundMatchBase> 
                            { 
                                new FoundMatchBase 
                                    { 
                                        Match = "BMW",  
                                        Definition = "plurilingüe"
                                    },
                                new FoundMatchBase 
                                    { 
                                        Match = "BMW",  
                                        Definition = "en inglés y alemán"
                                    }
                            },
                        false);
        }

        [Test]
        public void DefinitionEnclosedInParenthesesSingleResultContainingHyphenWithCheckedDefinition()
        {
            ExecuteTest(_defInParenthesesFormatString,
                        "El Protocolo FC-AL (Fibre Channel Arbitrated Loop)\r\n",
                        new List<FoundMatchBase> 
                            { 
                                new FoundMatchBase 
                                    { 
                                        Match = "FC-AL",  
                                        Definition = "Fibre Channel Arbitrated Loop"
                                    }
                            },
                        true);
        }

        [Test]
        public void DefinitionEnclosedInParenthesesMultipleResultsWithMixedDefinitions()
        {
            ExecuteTest(_defInParenthesesFormatString,
                        "El gen SRY (del inglés sex-determining region Y), descubierto en 1990, es un gen de determinación sexual.\r\n" +
                        "OLN\rOLN (Outdoor Life Network) es un canal de televisión por cable canadiense. " +
                        "RDX: Departamento de Salud y Servicios Humanos de EE.UU. (dominio público)." +
                        "RGB (en inglés Red, Green, Blue)" +
                        "SRI (Servicio de Rentas Internas), organismo público de Ecuador.\r" +
                        "* SRI (Space Research Institute: Instituto de Investigación Espacial).",
                        new List<FoundMatchBase> 
                            { 
                                new FoundMatchBase 
                                    { 
                                        Match = "SRY",  
                                        Definition = "del inglés sex-determining region Y"
                                    },
                                new FoundMatchBase 
                                    { 
                                        Match = "OLN",  
                                        Definition = "Outdoor Life Network"
                                    },
                                new FoundMatchBase 
                                    { 
                                        Match = "RGB",  
                                        Definition = "en inglés Red, Green, Blue"
                                    },
                                new FoundMatchBase 
                                    { 
                                        Match = "SRI",  
                                        Definition = "Servicio de Rentas Internas"
                                    },
                                new FoundMatchBase 
                                    { 
                                        Match = "SRI",  
                                        Definition = "Space Research Institute: Instituto de Investigación Espacial"
                                    }
                            },
                        true);
        }

        [Test]
        public void DefinitionEnclosedInParenthesesDefinitionNotValidIfJustAcronymMatches()
        {
            ExecuteTest(_defInParenthesesFormatString,
                        "Comparación entre SDTV y HDTV (La mitad izquierda es PAL, 575i, mientras que la mitad derecha, HDTV 1080i)", // no match expected
                        new List<FoundMatchBase>(), true);
        }

        #endregion Definition enclosed in parentheses

        #region Acronym enclosed in parentheses

        private const string _acrInParenthesesFormatString = @"(?<definition>\p{L}.*)\({acronymRE}\)";

        [Test]
        public void AcronymEnclosedInParenthesesNoResults()
        {
            ExecuteTest(_acrInParenthesesFormatString, "AAA\r\nAAA (puede) referirse.", new List<FoundMatchBase>(), true, true);
        }

        [Test]
        public void AcronymEnclosedInParenthesesInvalidDefinition()
        {
            ExecuteTest(_acrInParenthesesFormatString, "ICQ (OSCAR)", new List<FoundMatchBase>(), true, true);
        }

        [Test]
        public void AcronymEnclosedInParenthesesSingleResult()
        {
            ExecuteTest(_acrInParenthesesFormatString,
                        "SAP Developer Network\nSAP Developer network (SDN) es una comunidad en línea para desarrolladores SAP",
                        new List<FoundMatchBase>
                            {
                                new FoundMatchBase
                                    {
                                        Match = "SDN",
                                        Definition = "SAP Developer network"
                                    }
                            }, 
                        true, true);
        }

        [Test]
        public void AcronymEnclosedInParenthesesMultipleResults()
        {
            ExecuteTest(_acrInParenthesesFormatString,
                        "SAP Developer Network\nSAP developer Network (SDN) es una comunidad en línea para desarrolladores SAP\r\n"
                        + "Por ejemplo, Bellingham Linux user Group (BLUG), en Bellinham, Washington, es el organizador del evento anual Linuxfest Norhwest",
                        new List<FoundMatchBase>
                            {
                                new FoundMatchBase
                                    {
                                        Match = "SDN",
                                        Definition = "SAP developer Network"
                                    },
                                new FoundMatchBase
                                    {
                                        Match = "BLUG",
                                        Definition = "Bellingham Linux user Group"
                                    }
                            }, 
                        true, true);
        }
        
        [Test]
        public void AcronymEnclosedInParenthesesSingleCutResult()
        {
            ExecuteTest(_acrInParenthesesFormatString,
                        "Trotsky defendió medidas que buscaban acabar con el burocratismo dentro del Partido comunista de la Unión Soviética (PCUS)",
                        new List<FoundMatchBase>
                            {
                                new FoundMatchBase
                                    {
                                        Match = "PCUS",
                                        Definition = "Partido comunista de la Unión Soviética"
                                    }
                            },
                        true, true);
        }

        [Test]
        public void AcronymEnclosedInParenthesesMultipleCutResult()
        {
            ExecuteTest(_acrInParenthesesFormatString,
                        "Salud y medicina\r\n* Doctor de optometría (O.D.)\r\n"
                        + "* Enfermedad profesional, efectos en la salud de las exposiciones asociadas a una determinada ocupación.\r"
                        + "* Dexter Oculus, un término latino que significa \"ojo derecho\" (OCULUS siniestrasignifica el ojo izquierdo)." 
                        + " Abreviadas OD y OS, estos son los más\r\nDemanda biológica de oxígeno\nDemanda química de oxígeno\r"
                        + "La demanda química de oxígeno (DQO) es un parámetro que mide la cantidad de sustancias susceptibles de ser oxidadas "
                        + "por medios químicos que hay disueltas o en suspensión en una muestra líquida. Se utiliza para medir el grado de "
                        + "contaminación y se expresa en miligramos de oxígeno diatómico por litro (mgO2/l). Aunque este método pretende medir "
                        + "principalmente la concentración de materia orgánica, sufre interferencias por la presencia de sustancias inorgánicas "
                        + "susceptibles de ser oxidadas (sulfuros, sulfitos, yoduros...), que también se reflejan en la medida.\r\n"
                        + "*Estación de ferrocarril IBM, cerca de Greenock, Escocia\r*Intercontinental ballistic missile (ICBM), llamado el misil "
                        + "balístico intercontinental en español.",
                        new List<FoundMatchBase> ()
                            {
                                new FoundMatchBase
                                    {
                                        Match = "OD",
                                        Definition = "Doctor de optometría"
                                    },
                                new FoundMatchBase
                                    {
                                        Match = "DQO",
                                        Definition = "demanda química de oxígeno"
                                    }
                            },
                        true, true);
        }

        [Test]
        public void AcronymEnclosedInParenthesesSingleResultDoubleAsterisk()
        {
            ExecuteTest(_acrInParenthesesFormatString,
                        "**Dolby digital (DD): 48KHz, 448 kbit/s, hasta 5.1 canales.\r\n",
                        new List<FoundMatchBase>
                            {
                                new FoundMatchBase
                                    {
                                        Match = "DD",
                                        Definition = "Dolby digital"
                                    }
                            },
                        true, true);
        }

        [Test]
        public void AcronymEnclosedInParenthesesCorrectlyCutDefinition()
        {
            ExecuteTest(_acrInParenthesesFormatString,
                        "(e.j., Formato binario MPEG para XML, Unidades de fragmentos dos, sintaxis de descripción de lenguaje "
                        + "del flujo de bits (BSDL) y otros)\r\n"
                        + "Un Atlas Lingüístico y etnográfico de Andalucía (ALEA) es una obra compuesta por un conjunto de mapas "
                        + "lingüísticos, etnográficos y mixtos (de palabras y cosas) sobre las hablas meridionales de España\r\n"
                        + "sustituye a los utilizados anteriormente \"Número de identificación Bancaria\" (BIN).\n"
                        + "Para reducir la contaminación del aire, la \"California Air resources Board\" (CARB)\n\r"
                        + "alemana, Construcciones Aeronáuticas S.anónima (CASA)\r\n"
                        + "análisis de elementos finitos, dinámica de fluidos computacional (CFD)",
                        new List<FoundMatchBase>
                                {
                                    new FoundMatchBase
                                        {
                                            Match = "BSDL",
                                            Definition = "sintaxis de descripción de lenguaje del flujo de bits" 
                                        },
                                    new FoundMatchBase
                                        {
                                            Match = "ALEA",
                                            Definition = "Atlas Lingüístico y etnográfico de Andalucía" 
                                        },                                    
                                    new FoundMatchBase
                                        {
                                            Match = "BIN",
                                            Definition = "Número de identificación Bancaria" 
                                        },
                                    new FoundMatchBase
                                        {
                                            Match = "CARB",
                                            Definition = "California Air resources Board" 
                                        },
                                    new FoundMatchBase
                                        {
                                            Match = "CASA",
                                            Definition = "Construcciones Aeronáuticas S.anónima" 
                                        },
                                    new FoundMatchBase
                                        {
                                            Match = "CFD",
                                            Definition = "dinámica de fluidos computacional" 
                                        }
                                }, true, true);
        }

        [Test]
        public void AcronymEnclosedInParenthesesCorrectlyCutTrailingSuperfluousWords()
        {
            ExecuteTest(_acrInParenthesesFormatString,
                        "una unidad aritmético lógica (ALU)\r\n"
                        + "formato de archivo informático de diseño asistido por computadora (CAD)",
                        new List<FoundMatchBase>
                                {
                                    new FoundMatchBase
                                        {
                                            Match = "ALU",
                                            Definition = "unidad aritmético lógica" 
                                        },
                                    new FoundMatchBase
                                        {
                                            Match = "CAD",
                                            Definition = "diseño asistido por computadora" 
                                        }
                                }, true, true);
        }

        [Test]
        public void AcronymEnclosedInParenthesesDefinitionLimitedBySpecialCharacters()
        {
            ExecuteTest(_acrInParenthesesFormatString,
                        "El equipo se denomina \"Air Cargo explosivo Screener (ACES)\" y está dirigido fundamentalmente a contenedores de carga aérea o puertos.\r\n"
                        + "TIA: transitory ischaemic attack (TIA)\r"
                        + "Entrevistas telefónicas (CATI: Computer Assited Telephone Interview): Las encuestas telefónicas (CATI)" /* no match expected */,
                        new List<FoundMatchBase>
                                {
                                    new FoundMatchBase
                                        {
                                            Match = "ACES",
                                            Definition = "Air Cargo explosivo Screener" 
                                        },
                                    new FoundMatchBase
                                        {
                                            Match = "TIA",
                                            Definition = "transitory ischaemic attack" 
                                        }
                                }, true, true);
        }

        [Test]
        public void AcronymEnclosedInParenthesesDefinitionLimitedBySpecialWords()
        {
            ExecuteTest(_acrInParenthesesFormatString,
                        "transferencias de datos de alta velocidad a un sistema de televisión por cable (CATV) existente.\r\n" /* no match expected */
                        + "volvería un unicornio voraz (UV)\n"
                        + "un ultra moroso (UM)\r"
                        + "Otro tipo de molécula para cuya alteración no existe un umbral es el ácido desoxirribonucleico (ADN)", //no match expected
                        new List<FoundMatchBase>
                                {
                                    new FoundMatchBase
                                        {
                                            Match = "UV",
                                            Definition = "unicornio voraz" 
                                        },
                                    new FoundMatchBase
                                        {
                                            Match = "UM",
                                            Definition = "ultra moroso" 
                                        }
                                }, true, true);
        }

        [Test]
        public void AcronymEnclosedInParenthesesDefinitionStartAndEndCleanup()
        {
            ExecuteTest(_acrInParenthesesFormatString,
                        "La reunión del 75º aniversario de la sociedad meteorológica americana ((AMS) celebrada en Dallas en enero de 1995.",
                        new List<FoundMatchBase>
                                {
                                    new FoundMatchBase
                                        {
                                            Match = "AMS",
                                            Definition = "sociedad meteorológica americana" 
                                        }
                                }, true, true);
        }

        [Test]
        public void AcronymEnclosedInParenthesesRecursiveSearch()
        {
            ExecuteTest(_acrInParenthesesFormatString,
                        "El Gobierno de España a través de la Agencia española de medicamentos y productos sanitarios (AEMPS) anteriormente (AGEMED) "
                        + "aprobó el uso de la Risperidona en ancianos con demencia.\n"
                        + "Se propuso como estándar al Comité consultivo internacional para telegrafía y telefonía (CCITT) en 1984. El comité de normalización "
                        + "T1S1 de los Estados Unidos, acreditado por el Instituto americano de normalización (ANSI), realizó parte del trabajo preliminar "
                        + "sobre Frame Relay.\n\r"
                        + "ACIS es usado por varios sistemas de diseño asistido por computadora (CAD), fabricación asistida por computadora (CAM)",
                        new List<FoundMatchBase>
                                {
                                    new FoundMatchBase
                                        {
                                            Match = "AEMPS",
                                            Definition = "Agencia española de medicamentos y productos sanitarios" 
                                        },
                                    new FoundMatchBase
                                        {
                                            Match = "CCITT",
                                            Definition = "Comité consultivo internacional para telegrafía y telefonía" 
                                        },
                                    new FoundMatchBase
                                        {
                                            Match = "CAD",
                                            Definition = "diseño asistido por computadora" 
                                        }
                                }, true, true);
        }

        [Test]
        public void AcronymEnclosedInParenthesesExplicitDefinitionRecognition()
        {
            ExecuteTest(_acrInParenthesesFormatString,
                        "para entenderse Ent Versatile Disc (EVD)\r\n"
                        + "haciéndose cargo Atomic Energy of Can Limited (AECL)\r\n"
                        + "ardiente Alt Header (AH)\n"
                        + "banda ancha Broadband Global All Network (BGAN)\r"
                        + "Site de liberasion Bibliothèque Le Jacques Doucet (BLJD)\r\n"
                        + "En Rumania, Banca Comercială Rôs (BCR), principal banco del país\r"
                        + "Alemania, Construcciones Aeronáuticas S.A. (CASA)\r\n",
                        new List<FoundMatchBase>
                                {
                                    new FoundMatchBase
                                        {
                                            Match = "EVD",
                                            Definition = "Ent Versatile Disc" 
                                        },
                                    new FoundMatchBase
                                        {
                                            Match = "AECL",
                                            Definition = "Atomic Energy of Can Limited" 
                                        },
                                    new FoundMatchBase
                                        {
                                            Match = "AH",
                                            Definition = "Alt Header" 
                                        },
                                    new FoundMatchBase
                                        {
                                            Match = "BGAN",
                                            Definition = "Broadband Global All Network" 
                                        }
                                    ,
                                    new FoundMatchBase
                                        {
                                            Match = "BLJD",
                                            Definition = "Bibliothèque Le Jacques Doucet" 
                                        },
                                    new FoundMatchBase
                                        {
                                            Match = "BCR",
                                            Definition = "Banca Comercială Rôs" 
                                        },
                                    new FoundMatchBase
                                        {
                                            Match = "CASA",
                                            Definition = "Construcciones Aeronáuticas S.A." 
                                        }
                                }, true, true);
        }

        [Test]
        public void AcronymEnclosedInParenthesesRemoveAccentsInDefinitionWords()
        {
            ExecuteTest(_acrInParenthesesFormatString,
                        "Abuja, Nigeria, se celebró la I Cumbre América del sur-África (ASA)\r\n"
                        + "Es administrado por el Centre international d'études pédagogiques (CIEP)",
                        new List<FoundMatchBase>
                                {
                                    new FoundMatchBase
                                        {
                                            Match = "ASA",
                                            Definition = "América del sur-África" 
                                        },
                                    new FoundMatchBase
                                        {
                                            Match = "CIEP",
                                            Definition = "Centre international d'études pédagogiques" 
                                        }
                                }, true, true);
        }

        [Test]
        public void AcronymEnclosedInParenthesesUseAllCapsToRecognizeDefinitionAndNotJustInitials()
        {
            ExecuteTest(_acrInParenthesesFormatString,
                        "el puerto del switch que tenga un menor Bridge ID (BID).",
                        new List<FoundMatchBase>
                                {
                                    new FoundMatchBase
                                        {
                                            Match = "BID",
                                            Definition = "Bridge ID" 
                                        }
                                }, true, true);
        }

        [Test]
        public void AcronymEnclosedInParenthesesBreakDefinitionPartsByAllPossibleCharacters()
        {
            ExecuteTest(_acrInParenthesesFormatString,
                        "Site de l'Institut national de l'histoire de l'art (INHA).\n"
                        + "tras 7 fusiones y 2 intervenciones (Caja castilla La Mancha (CCM)\r"
                        + "Composite capability/Preference Profiles (CC/PP)",
                        new List<FoundMatchBase>
                                {
                                    new FoundMatchBase
                                        {
                                            Match = "INHA",
                                            Definition = "Institut national de l'histoire de l'art" 
                                        },
                                    new FoundMatchBase
                                        {
                                            Match = "CCM",
                                            Definition = "Caja castilla La Mancha" 
                                        },
                                    new FoundMatchBase
                                        {
                                            Match = "CC/PP",
                                            Definition = "Composite capability/Preference Profiles" 
                                        }
                                }, true, true);
        }

        [Test]
        public void AcronymEnclosedInParenthesesSatisfyAcronymIfWordLongEnough()
        {
            ExecuteTest(_acrInParenthesesFormatString,
                        "posteriormente quedaron convertidos en colegios de educación infantil y primaria (CEIP).\r\n"
                        + "Es administrado por el Centre international d'etudes pédagogiques (CIEP)",
                        new List<FoundMatchBase>
                                {
                                    new FoundMatchBase
                                        {
                                            Match = "CEIP",
                                            Definition = "colegios de educación infantil y primaria" 
                                        },
                                    new FoundMatchBase
                                        {
                                            Match = "CIEP",
                                            Definition = "Centre international d'etudes pédagogiques" 
                                        }
                                }, true, true);
        }

        [Test]
        public void AcronymEnclosedInParenthesesSearchDefinitionJustInPrecedingPhrase()
        {
            ExecuteTest(_acrInParenthesesFormatString,
                        "Poliadenilación: es la adición al extremo 3' de una cola poli-A, una secuencia larga de poliadenilato, " 
                        + "es decir, un tramo de RNA cuyas bases son todas adenina. Su adición está mediada por una secuencia o "
                        + "señal de poliadenilación (AAUAAA)\r\n" // no match expected 
                        + "Complejos: Combinaciones entre cualquiera de las clases anteriores, sin ningún patrón de orden definido. "
                        + "ej: (ACC)8+TG+(GA)12+(TTA)5+GC+(TTA)4\n" // no match expected 
                        + "Sería reemplazado por Francis Collins en abril de 1993, en gran parte por su enemistad con Bernadine Healy "
                        + "que era su jefe por aquel entonces. Tras esto el nombre del Centro cambió a Instituto Nacional de Investigaciones "
                        + "del Genoma Humano (NHGRI).", // no match expected
                        new List<FoundMatchBase>(), true, true);
        }        

        [Test]
        public void AcronymEnclosedInParenthesesExplicitDefinitionFirstWordInitialNotCapital()
        {
            ExecuteTest(_acrInParenthesesFormatString,
                        "Mastering eXtended Markup Language (XML)",
                        new List<FoundMatchBase>
                                {
                                    new FoundMatchBase
                                        {
                                            Match = "XML",
                                            Definition = "eXtended Markup Language" 
                                        }
                                }, true, true);
        }

        [Test]
        public void AcronymEnclosedInParenthesesExplicitDefinitionNotUsingAPartOfAnotherAcronymNotBeingTheInitial()
        {
            ExecuteTest(_acrInParenthesesFormatString,
                        "El AES fue anunciado por el Instituto Nacional de Estándares y Tecnología (NIST)\r\n" // no match expected
                        + "GLG is Not Unique (GNU)\n"
                        + "Giants is LAN Unique (GLANU)",
                        new List<FoundMatchBase> 
                                {
                                    new FoundMatchBase
                                        {
                                            Match = "GNU",
                                            Definition = "GLG is Not Unique" 
                                        },
                                    new FoundMatchBase
                                        {
                                            Match = "GLANU",
                                            Definition = "Giants is LAN Unique" 
                                        }
                                }, true, true);
        }        

        #endregion Acronym enclosed in parentheses

        #region Acronym followed by colon

        private const string _acrFollowedByColonFormatString = @"{acronymRE}\:\s+(?<definition>\p{L}.*)";

        [Test]
        public void AcronymFollowedByColonNoResults()
        {
            ExecuteTest(_acrFollowedByColonFormatString,
                        "Protocolo Fibre Channel Switched, usado en switches, en este caso varias comunicaciones pueden ocurrir simultáneamente.", 
                        new List<FoundMatchBase>(), true, true);
        }

        [Test]
        public void AcronymFollowedByColonInvalidDefinition()
        {
            ExecuteTest(_acrFollowedByColonFormatString,
                        "FC-SW: Protocolo Fibre Channel Switched, usado en switches, en este caso varias comunicaciones pueden ocurrir simultáneamente.",
                        new List<FoundMatchBase>(), true, true);
        }

        [Test]
        public void AcronymFollowedByColonSingleResult()
        {
            ExecuteTest(_acrFollowedByColonFormatString,
                        "FC-AL: Fibre channel Arbitrated Loop, usado en hubs, en el SAN hub este protocolo es el que se usa por excelencia.",
                        new List<FoundMatchBase>
                            {
                                new FoundMatchBase
                                    {
                                        Match = "FC-AL",
                                        Definition = "Fibre channel Arbitrated Loop"
                                    }
                            },
                        true, false, true);
        }

        [Test]
        public void AcronymFollowedByColonSingleResultWithNotRigidCheck()
        {
            ExecuteTest(_acrFollowedByColonFormatString,
                        "FC-AL: Protocolo fibre Channel Arbitrated Loop, usado en hubs, en el SAN hub este protocolo es el que se usa por excelencia.",
                        new List<FoundMatchBase>
                            {
                                new FoundMatchBase
                                    {
                                        Match = "FC-AL",
                                        Definition = "Protocolo fibre Channel Arbitrated Loop"
                                    }
                            },
                        true, false, true);
        }

        [Test]
        public void AcronymFollowedByColonMultipleResults()
        {
            ExecuteTest(_acrFollowedByColonFormatString,
                        "FC-AL: Protocolo Fibre Channel Arbitrated Loop, usado en hubs, en el SAN hub este protocolo es el que se usa por excelencia.\r\n"
                        + "TUG: Grupo de usuarios de TeX ululantes",
                        new List<FoundMatchBase>
                            {
                                new FoundMatchBase
                                    {
                                        Match = "FC-AL",
                                        Definition = "Protocolo Fibre Channel Arbitrated Loop"
                                    },
                                new FoundMatchBase
                                    {
                                        Match = "TUG",
                                        Definition = "Grupo de usuarios de TeX"
                                    }
                            },
                        true, false, true);
        }

        [Test]
        public void AcronymFollowedByColonLimitDefinitionToOnePhrase()
        {
            ExecuteTest(_acrFollowedByColonFormatString,
                        "En VV. AA.: El universo, enciclopedia de la astronomía y el espacio. Buenos Aires: Planeta-De Agostini", // no match expected
                        new List<FoundMatchBase>(), true, false, true);
        }

        [Test]
        public void AcronymFollowedByColonLimitDefinitionInNumberOfWords()
        {
            ExecuteTest(_acrFollowedByColonFormatString,
                        "Estadios en el BDSM: se identifica con esa denominación a las etapas por las que frecuentemente pasan las "
                        + "personas que se acercan a la comunidad de Bebedores Daltónicos Sin Mesura", // no match expected
                        new List<FoundMatchBase>(), true, false, true);
        }
        
        #endregion Acronym followed by colon

        #region AnyPattern

        [Test]
        public void AnyPatternAvoidGivingMatchAsDefinition()
        {
            ExecuteTest(_acrFollowedByColonFormatString,
                        "Códigos INSEE: INSEE da un código numérico a varias entidades en Francia", // no match expected
                        new List<FoundMatchBase>(), true, false, true);
        }

        [Test]
        public void AnyPatternPreferExplicitOverImplicit()
        {
            ExecuteTest(_acrInParenthesesFormatString,
                        "Diccionario de la lengua española de la Real Academia Española (DRAE)",
                        new List<FoundMatchBase>
                            {
                                new FoundMatchBase
                                    {
                                        Match = "DRAE",
                                        Definition = "Diccionario de la lengua española de la Real Academia Española"
                                    }
                            }, true, true);
        }

        [Test]
        public void AnyPatternCleanSurroundingCharacters()
        {
            ExecuteTest(_defInParenthesesFormatString,
                        "ISBN (\"International Standard Book Number\")\n"
                        + "PGB ('partido de la gente Del bar')\n"
                        + "MA (<<mamporreros asociados>>)\n", 
                        new List<FoundMatchBase>
                            {
                                new FoundMatchBase
                                    {
                                        Match = "ISBN",
                                        Definition = "International Standard Book Number"
                                    },
                                new FoundMatchBase
                                    {
                                        Match = "PGB",
                                        Definition = "partido de la gente Del bar"
                                    },
                                new FoundMatchBase
                                    {
                                        Match = "MA",
                                        Definition = "mamporreros asociados"
                                    }
                            }, true);
        }
        
        [Test]
        public void AnyPatternAvoidGivingStringContainingMatchAsDefinition()
        {
            ExecuteTest(_defInParenthesesFormatString,
                        "Java ME (antes J2ME)\n"
                        + "Programas de  Windows XP (Plus! Para Windows XP y Plus! Digital Media Edition)\r\n"
                        + "Reed Exhibition Companies, S.A. de C.V. (actualmente Representaciones de Exposiciones "
                        + "México, S.A. de C.V.)", 
                        new List<FoundMatchBase>(), true);
        }

        [Test]
        public void AnyPatternLimitNumberOfWordsOfDefinition()
        {
            ExecuteTest(_acrInParenthesesFormatString,
                        "Esta nueva línea de razonamiento de balance estratégico, en plena Guerra Fría, supone que se necesitarían "
                        + "en el futuro, construir mayor cantidad de misiles ICBM con múltiples ojivas nucleares, que son de alto costo "
                        + "de producción y mantenimiento, para poder atacar al enemigo y superar los misiles defensivos (ABM)\r\n" // no match expected
                        + "Dado que las prostaglandinas (PG) participan en las respuestas inflamatorias espectóricas al estimular las terminales "
                        + "nerviosas del dolor, los antiinflamatorios no esteroides (AINE)", // no match expected
                        new List<FoundMatchBase>(), true, true);

            ExecuteTest(_defInParenthesesFormatString,
                        "CFA (al menos el 65% de las posiciones en reservas depositadas en el Tesoro francés, donde se les concede "
                        + "una garantía de tipos de cambio)\n"
                        + "DOP (Dizionario d'ortografia e di pronunzia, Diccionario de ortografía y pronunciación de la lengua italiana)\r\n"
                        + "EPC (el cambio de nombre es debido a que desarrollan su formación en este centro agentes rurales, funcionarios "
                        + "de prisiones, bomberos, seguridad privada, agentes forestales, además de los mossos y Policías Locales)",
                        new List<FoundMatchBase>(), true);
        }

        #endregion AnyPattern
    }
}
