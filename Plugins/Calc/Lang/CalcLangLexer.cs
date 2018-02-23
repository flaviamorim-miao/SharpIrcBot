//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//     ANTLR Version: 4.7.1
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

// Generated from .\CalcLang.g4 by ANTLR 4.7.1

// Unreachable code detected
#pragma warning disable 0162
// The variable '...' is assigned but its value is never used
#pragma warning disable 0219
// Missing XML comment for publicly visible type or member '...'
#pragma warning disable 1591
// Ambiguous reference in cref attribute
#pragma warning disable 419

namespace SharpIrcBot.Plugins.Calc.Language {
using System;
using System.IO;
using System.Text;
using Antlr4.Runtime;
using Antlr4.Runtime.Atn;
using Antlr4.Runtime.Misc;
using DFA = Antlr4.Runtime.Dfa.DFA;

[System.CodeDom.Compiler.GeneratedCode("ANTLR", "4.7.1")]
[System.CLSCompliant(false)]
public partial class CalcLangLexer : Lexer {
	protected static DFA[] decisionToDFA;
	protected static PredictionContextCache sharedContextCache = new PredictionContextCache();
	public const int
		T__0=1, T__1=2, T__2=3, T__3=4, T__4=5, T__5=6, T__6=7, T__7=8, T__8=9, 
		T__9=10, T__10=11, T__11=12, Whitespaces=13, Decimal=14, Identifier=15, 
		Integer=16, Integer10=17, Integer16=18, Integer8=19, Integer2=20;
	public static string[] channelNames = {
		"DEFAULT_TOKEN_CHANNEL", "HIDDEN"
	};

	public static string[] modeNames = {
		"DEFAULT_MODE"
	};

	public static readonly string[] ruleNames = {
		"T__0", "T__1", "T__2", "T__3", "T__4", "T__5", "T__6", "T__7", "T__8", 
		"T__9", "T__10", "T__11", "Whitespaces", "Decimal", "Identifier", "Integer", 
		"Integer10", "Integer16", "Integer8", "Integer2", "Whitespace"
	};


	public CalcLangLexer(ICharStream input)
	: this(input, Console.Out, Console.Error) { }

	public CalcLangLexer(ICharStream input, TextWriter output, TextWriter errorOutput)
	: base(input, output, errorOutput)
	{
		Interpreter = new LexerATNSimulator(this, _ATN, decisionToDFA, sharedContextCache);
	}

	private static readonly string[] _LiteralNames = {
		null, "'('", "')'", "'-'", "'**'", "'*'", "'/'", "'%'", "'+'", "'&'", 
		"'^'", "'|'", "','"
	};
	private static readonly string[] _SymbolicNames = {
		null, null, null, null, null, null, null, null, null, null, null, null, 
		null, "Whitespaces", "Decimal", "Identifier", "Integer", "Integer10", 
		"Integer16", "Integer8", "Integer2"
	};
	public static readonly IVocabulary DefaultVocabulary = new Vocabulary(_LiteralNames, _SymbolicNames);

	[NotNull]
	public override IVocabulary Vocabulary
	{
		get
		{
			return DefaultVocabulary;
		}
	}

	public override string GrammarFileName { get { return "CalcLang.g4"; } }

	public override string[] RuleNames { get { return ruleNames; } }

	public override string[] ChannelNames { get { return channelNames; } }

	public override string[] ModeNames { get { return modeNames; } }

	public override string SerializedAtn { get { return new string(_serializedATN); } }

	static CalcLangLexer() {
		decisionToDFA = new DFA[_ATN.NumberOfDecisions];
		for (int i = 0; i < _ATN.NumberOfDecisions; i++) {
			decisionToDFA[i] = new DFA(_ATN.GetDecisionState(i), i);
		}
	}
	private static char[] _serializedATN = {
		'\x3', '\x608B', '\xA72A', '\x8133', '\xB9ED', '\x417C', '\x3BE7', '\x7786', 
		'\x5964', '\x2', '\x16', '\x84', '\b', '\x1', '\x4', '\x2', '\t', '\x2', 
		'\x4', '\x3', '\t', '\x3', '\x4', '\x4', '\t', '\x4', '\x4', '\x5', '\t', 
		'\x5', '\x4', '\x6', '\t', '\x6', '\x4', '\a', '\t', '\a', '\x4', '\b', 
		'\t', '\b', '\x4', '\t', '\t', '\t', '\x4', '\n', '\t', '\n', '\x4', '\v', 
		'\t', '\v', '\x4', '\f', '\t', '\f', '\x4', '\r', '\t', '\r', '\x4', '\xE', 
		'\t', '\xE', '\x4', '\xF', '\t', '\xF', '\x4', '\x10', '\t', '\x10', '\x4', 
		'\x11', '\t', '\x11', '\x4', '\x12', '\t', '\x12', '\x4', '\x13', '\t', 
		'\x13', '\x4', '\x14', '\t', '\x14', '\x4', '\x15', '\t', '\x15', '\x4', 
		'\x16', '\t', '\x16', '\x3', '\x2', '\x3', '\x2', '\x3', '\x3', '\x3', 
		'\x3', '\x3', '\x4', '\x3', '\x4', '\x3', '\x5', '\x3', '\x5', '\x3', 
		'\x5', '\x3', '\x6', '\x3', '\x6', '\x3', '\a', '\x3', '\a', '\x3', '\b', 
		'\x3', '\b', '\x3', '\t', '\x3', '\t', '\x3', '\n', '\x3', '\n', '\x3', 
		'\v', '\x3', '\v', '\x3', '\f', '\x3', '\f', '\x3', '\r', '\x3', '\r', 
		'\x3', '\xE', '\x6', '\xE', 'H', '\n', '\xE', '\r', '\xE', '\xE', '\xE', 
		'I', '\x3', '\xE', '\x3', '\xE', '\x3', '\xF', '\x6', '\xF', 'O', '\n', 
		'\xF', '\r', '\xF', '\xE', '\xF', 'P', '\x3', '\xF', '\x3', '\xF', '\x6', 
		'\xF', 'U', '\n', '\xF', '\r', '\xF', '\xE', '\xF', 'V', '\x3', '\x10', 
		'\x3', '\x10', '\a', '\x10', '[', '\n', '\x10', '\f', '\x10', '\xE', '\x10', 
		'^', '\v', '\x10', '\x3', '\x11', '\x3', '\x11', '\x3', '\x11', '\x3', 
		'\x11', '\x5', '\x11', '\x64', '\n', '\x11', '\x3', '\x12', '\x6', '\x12', 
		'g', '\n', '\x12', '\r', '\x12', '\xE', '\x12', 'h', '\x3', '\x13', '\x3', 
		'\x13', '\x3', '\x13', '\x3', '\x13', '\x6', '\x13', 'o', '\n', '\x13', 
		'\r', '\x13', '\xE', '\x13', 'p', '\x3', '\x14', '\x3', '\x14', '\x3', 
		'\x14', '\x3', '\x14', '\x6', '\x14', 'w', '\n', '\x14', '\r', '\x14', 
		'\xE', '\x14', 'x', '\x3', '\x15', '\x3', '\x15', '\x3', '\x15', '\x3', 
		'\x15', '\x6', '\x15', '\x7F', '\n', '\x15', '\r', '\x15', '\xE', '\x15', 
		'\x80', '\x3', '\x16', '\x3', '\x16', '\x2', '\x2', '\x17', '\x3', '\x3', 
		'\x5', '\x4', '\a', '\x5', '\t', '\x6', '\v', '\a', '\r', '\b', '\xF', 
		'\t', '\x11', '\n', '\x13', '\v', '\x15', '\f', '\x17', '\r', '\x19', 
		'\xE', '\x1B', '\xF', '\x1D', '\x10', '\x1F', '\x11', '!', '\x12', '#', 
		'\x13', '%', '\x14', '\'', '\x15', ')', '\x16', '+', '\x2', '\x3', '\x2', 
		'\n', '\x3', '\x2', '\x32', ';', '\x4', '\x2', '\x43', '\\', '\x63', '|', 
		'\x6', '\x2', '\x32', ';', '\x43', '\\', '\x61', '\x61', '\x63', '|', 
		'\x4', '\x2', '\x32', ';', '\x61', '\x61', '\x6', '\x2', '\x32', ';', 
		'\x43', 'H', '\x61', '\x61', '\x63', 'h', '\x4', '\x2', '\x32', '\x39', 
		'\x61', '\x61', '\x4', '\x2', '\x32', '\x33', '\x61', '\x61', '\x4', '\x2', 
		'\v', '\xF', '\"', '\"', '\x2', '\x8D', '\x2', '\x3', '\x3', '\x2', '\x2', 
		'\x2', '\x2', '\x5', '\x3', '\x2', '\x2', '\x2', '\x2', '\a', '\x3', '\x2', 
		'\x2', '\x2', '\x2', '\t', '\x3', '\x2', '\x2', '\x2', '\x2', '\v', '\x3', 
		'\x2', '\x2', '\x2', '\x2', '\r', '\x3', '\x2', '\x2', '\x2', '\x2', '\xF', 
		'\x3', '\x2', '\x2', '\x2', '\x2', '\x11', '\x3', '\x2', '\x2', '\x2', 
		'\x2', '\x13', '\x3', '\x2', '\x2', '\x2', '\x2', '\x15', '\x3', '\x2', 
		'\x2', '\x2', '\x2', '\x17', '\x3', '\x2', '\x2', '\x2', '\x2', '\x19', 
		'\x3', '\x2', '\x2', '\x2', '\x2', '\x1B', '\x3', '\x2', '\x2', '\x2', 
		'\x2', '\x1D', '\x3', '\x2', '\x2', '\x2', '\x2', '\x1F', '\x3', '\x2', 
		'\x2', '\x2', '\x2', '!', '\x3', '\x2', '\x2', '\x2', '\x2', '#', '\x3', 
		'\x2', '\x2', '\x2', '\x2', '%', '\x3', '\x2', '\x2', '\x2', '\x2', '\'', 
		'\x3', '\x2', '\x2', '\x2', '\x2', ')', '\x3', '\x2', '\x2', '\x2', '\x3', 
		'-', '\x3', '\x2', '\x2', '\x2', '\x5', '/', '\x3', '\x2', '\x2', '\x2', 
		'\a', '\x31', '\x3', '\x2', '\x2', '\x2', '\t', '\x33', '\x3', '\x2', 
		'\x2', '\x2', '\v', '\x36', '\x3', '\x2', '\x2', '\x2', '\r', '\x38', 
		'\x3', '\x2', '\x2', '\x2', '\xF', ':', '\x3', '\x2', '\x2', '\x2', '\x11', 
		'<', '\x3', '\x2', '\x2', '\x2', '\x13', '>', '\x3', '\x2', '\x2', '\x2', 
		'\x15', '@', '\x3', '\x2', '\x2', '\x2', '\x17', '\x42', '\x3', '\x2', 
		'\x2', '\x2', '\x19', '\x44', '\x3', '\x2', '\x2', '\x2', '\x1B', 'G', 
		'\x3', '\x2', '\x2', '\x2', '\x1D', 'N', '\x3', '\x2', '\x2', '\x2', '\x1F', 
		'X', '\x3', '\x2', '\x2', '\x2', '!', '\x63', '\x3', '\x2', '\x2', '\x2', 
		'#', '\x66', '\x3', '\x2', '\x2', '\x2', '%', 'j', '\x3', '\x2', '\x2', 
		'\x2', '\'', 'r', '\x3', '\x2', '\x2', '\x2', ')', 'z', '\x3', '\x2', 
		'\x2', '\x2', '+', '\x82', '\x3', '\x2', '\x2', '\x2', '-', '.', '\a', 
		'*', '\x2', '\x2', '.', '\x4', '\x3', '\x2', '\x2', '\x2', '/', '\x30', 
		'\a', '+', '\x2', '\x2', '\x30', '\x6', '\x3', '\x2', '\x2', '\x2', '\x31', 
		'\x32', '\a', '/', '\x2', '\x2', '\x32', '\b', '\x3', '\x2', '\x2', '\x2', 
		'\x33', '\x34', '\a', ',', '\x2', '\x2', '\x34', '\x35', '\a', ',', '\x2', 
		'\x2', '\x35', '\n', '\x3', '\x2', '\x2', '\x2', '\x36', '\x37', '\a', 
		',', '\x2', '\x2', '\x37', '\f', '\x3', '\x2', '\x2', '\x2', '\x38', '\x39', 
		'\a', '\x31', '\x2', '\x2', '\x39', '\xE', '\x3', '\x2', '\x2', '\x2', 
		':', ';', '\a', '\'', '\x2', '\x2', ';', '\x10', '\x3', '\x2', '\x2', 
		'\x2', '<', '=', '\a', '-', '\x2', '\x2', '=', '\x12', '\x3', '\x2', '\x2', 
		'\x2', '>', '?', '\a', '(', '\x2', '\x2', '?', '\x14', '\x3', '\x2', '\x2', 
		'\x2', '@', '\x41', '\a', '`', '\x2', '\x2', '\x41', '\x16', '\x3', '\x2', 
		'\x2', '\x2', '\x42', '\x43', '\a', '~', '\x2', '\x2', '\x43', '\x18', 
		'\x3', '\x2', '\x2', '\x2', '\x44', '\x45', '\a', '.', '\x2', '\x2', '\x45', 
		'\x1A', '\x3', '\x2', '\x2', '\x2', '\x46', 'H', '\x5', '+', '\x16', '\x2', 
		'G', '\x46', '\x3', '\x2', '\x2', '\x2', 'H', 'I', '\x3', '\x2', '\x2', 
		'\x2', 'I', 'G', '\x3', '\x2', '\x2', '\x2', 'I', 'J', '\x3', '\x2', '\x2', 
		'\x2', 'J', 'K', '\x3', '\x2', '\x2', '\x2', 'K', 'L', '\b', '\xE', '\x2', 
		'\x2', 'L', '\x1C', '\x3', '\x2', '\x2', '\x2', 'M', 'O', '\t', '\x2', 
		'\x2', '\x2', 'N', 'M', '\x3', '\x2', '\x2', '\x2', 'O', 'P', '\x3', '\x2', 
		'\x2', '\x2', 'P', 'N', '\x3', '\x2', '\x2', '\x2', 'P', 'Q', '\x3', '\x2', 
		'\x2', '\x2', 'Q', 'R', '\x3', '\x2', '\x2', '\x2', 'R', 'T', '\a', '\x30', 
		'\x2', '\x2', 'S', 'U', '\t', '\x2', '\x2', '\x2', 'T', 'S', '\x3', '\x2', 
		'\x2', '\x2', 'U', 'V', '\x3', '\x2', '\x2', '\x2', 'V', 'T', '\x3', '\x2', 
		'\x2', '\x2', 'V', 'W', '\x3', '\x2', '\x2', '\x2', 'W', '\x1E', '\x3', 
		'\x2', '\x2', '\x2', 'X', '\\', '\t', '\x3', '\x2', '\x2', 'Y', '[', '\t', 
		'\x4', '\x2', '\x2', 'Z', 'Y', '\x3', '\x2', '\x2', '\x2', '[', '^', '\x3', 
		'\x2', '\x2', '\x2', '\\', 'Z', '\x3', '\x2', '\x2', '\x2', '\\', ']', 
		'\x3', '\x2', '\x2', '\x2', ']', ' ', '\x3', '\x2', '\x2', '\x2', '^', 
		'\\', '\x3', '\x2', '\x2', '\x2', '_', '\x64', '\x5', '#', '\x12', '\x2', 
		'`', '\x64', '\x5', '%', '\x13', '\x2', '\x61', '\x64', '\x5', '\'', '\x14', 
		'\x2', '\x62', '\x64', '\x5', ')', '\x15', '\x2', '\x63', '_', '\x3', 
		'\x2', '\x2', '\x2', '\x63', '`', '\x3', '\x2', '\x2', '\x2', '\x63', 
		'\x61', '\x3', '\x2', '\x2', '\x2', '\x63', '\x62', '\x3', '\x2', '\x2', 
		'\x2', '\x64', '\"', '\x3', '\x2', '\x2', '\x2', '\x65', 'g', '\t', '\x5', 
		'\x2', '\x2', '\x66', '\x65', '\x3', '\x2', '\x2', '\x2', 'g', 'h', '\x3', 
		'\x2', '\x2', '\x2', 'h', '\x66', '\x3', '\x2', '\x2', '\x2', 'h', 'i', 
		'\x3', '\x2', '\x2', '\x2', 'i', '$', '\x3', '\x2', '\x2', '\x2', 'j', 
		'k', '\a', '\x32', '\x2', '\x2', 'k', 'l', '\a', 'z', '\x2', '\x2', 'l', 
		'n', '\x3', '\x2', '\x2', '\x2', 'm', 'o', '\t', '\x6', '\x2', '\x2', 
		'n', 'm', '\x3', '\x2', '\x2', '\x2', 'o', 'p', '\x3', '\x2', '\x2', '\x2', 
		'p', 'n', '\x3', '\x2', '\x2', '\x2', 'p', 'q', '\x3', '\x2', '\x2', '\x2', 
		'q', '&', '\x3', '\x2', '\x2', '\x2', 'r', 's', '\a', '\x32', '\x2', '\x2', 
		's', 't', '\a', 'q', '\x2', '\x2', 't', 'v', '\x3', '\x2', '\x2', '\x2', 
		'u', 'w', '\t', '\a', '\x2', '\x2', 'v', 'u', '\x3', '\x2', '\x2', '\x2', 
		'w', 'x', '\x3', '\x2', '\x2', '\x2', 'x', 'v', '\x3', '\x2', '\x2', '\x2', 
		'x', 'y', '\x3', '\x2', '\x2', '\x2', 'y', '(', '\x3', '\x2', '\x2', '\x2', 
		'z', '{', '\a', '\x32', '\x2', '\x2', '{', '|', '\a', '\x64', '\x2', '\x2', 
		'|', '~', '\x3', '\x2', '\x2', '\x2', '}', '\x7F', '\t', '\b', '\x2', 
		'\x2', '~', '}', '\x3', '\x2', '\x2', '\x2', '\x7F', '\x80', '\x3', '\x2', 
		'\x2', '\x2', '\x80', '~', '\x3', '\x2', '\x2', '\x2', '\x80', '\x81', 
		'\x3', '\x2', '\x2', '\x2', '\x81', '*', '\x3', '\x2', '\x2', '\x2', '\x82', 
		'\x83', '\t', '\t', '\x2', '\x2', '\x83', ',', '\x3', '\x2', '\x2', '\x2', 
		'\f', '\x2', 'I', 'P', 'V', '\\', '\x63', 'h', 'p', 'x', '\x80', '\x3', 
		'\x2', '\x3', '\x2',
	};

	public static readonly ATN _ATN =
		new ATNDeserializer().Deserialize(_serializedATN);


}
} // namespace SharpIrcBot.Plugins.Calc.Language
