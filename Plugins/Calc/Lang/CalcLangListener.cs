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
using Antlr4.Runtime.Misc;
using IParseTreeListener = Antlr4.Runtime.Tree.IParseTreeListener;
using IToken = Antlr4.Runtime.IToken;

/// <summary>
/// This interface defines a complete listener for a parse tree produced by
/// <see cref="CalcLangParser"/>.
/// </summary>
[System.CodeDom.Compiler.GeneratedCode("ANTLR", "4.7.1")]
[System.CLSCompliant(false)]
public interface ICalcLangListener : IParseTreeListener {
	/// <summary>
	/// Enter a parse tree produced by the <c>Div</c>
	/// labeled alternative in <see cref="CalcLangParser.expression"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	void EnterDiv([NotNull] CalcLangParser.DivContext context);
	/// <summary>
	/// Exit a parse tree produced by the <c>Div</c>
	/// labeled alternative in <see cref="CalcLangParser.expression"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	void ExitDiv([NotNull] CalcLangParser.DivContext context);
	/// <summary>
	/// Enter a parse tree produced by the <c>Add</c>
	/// labeled alternative in <see cref="CalcLangParser.expression"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	void EnterAdd([NotNull] CalcLangParser.AddContext context);
	/// <summary>
	/// Exit a parse tree produced by the <c>Add</c>
	/// labeled alternative in <see cref="CalcLangParser.expression"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	void ExitAdd([NotNull] CalcLangParser.AddContext context);
	/// <summary>
	/// Enter a parse tree produced by the <c>Neg</c>
	/// labeled alternative in <see cref="CalcLangParser.expression"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	void EnterNeg([NotNull] CalcLangParser.NegContext context);
	/// <summary>
	/// Exit a parse tree produced by the <c>Neg</c>
	/// labeled alternative in <see cref="CalcLangParser.expression"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	void ExitNeg([NotNull] CalcLangParser.NegContext context);
	/// <summary>
	/// Enter a parse tree produced by the <c>Sub</c>
	/// labeled alternative in <see cref="CalcLangParser.expression"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	void EnterSub([NotNull] CalcLangParser.SubContext context);
	/// <summary>
	/// Exit a parse tree produced by the <c>Sub</c>
	/// labeled alternative in <see cref="CalcLangParser.expression"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	void ExitSub([NotNull] CalcLangParser.SubContext context);
	/// <summary>
	/// Enter a parse tree produced by the <c>Dec</c>
	/// labeled alternative in <see cref="CalcLangParser.expression"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	void EnterDec([NotNull] CalcLangParser.DecContext context);
	/// <summary>
	/// Exit a parse tree produced by the <c>Dec</c>
	/// labeled alternative in <see cref="CalcLangParser.expression"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	void ExitDec([NotNull] CalcLangParser.DecContext context);
	/// <summary>
	/// Enter a parse tree produced by the <c>Func</c>
	/// labeled alternative in <see cref="CalcLangParser.expression"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	void EnterFunc([NotNull] CalcLangParser.FuncContext context);
	/// <summary>
	/// Exit a parse tree produced by the <c>Func</c>
	/// labeled alternative in <see cref="CalcLangParser.expression"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	void ExitFunc([NotNull] CalcLangParser.FuncContext context);
	/// <summary>
	/// Enter a parse tree produced by the <c>Cst</c>
	/// labeled alternative in <see cref="CalcLangParser.expression"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	void EnterCst([NotNull] CalcLangParser.CstContext context);
	/// <summary>
	/// Exit a parse tree produced by the <c>Cst</c>
	/// labeled alternative in <see cref="CalcLangParser.expression"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	void ExitCst([NotNull] CalcLangParser.CstContext context);
	/// <summary>
	/// Enter a parse tree produced by the <c>Mul</c>
	/// labeled alternative in <see cref="CalcLangParser.expression"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	void EnterMul([NotNull] CalcLangParser.MulContext context);
	/// <summary>
	/// Exit a parse tree produced by the <c>Mul</c>
	/// labeled alternative in <see cref="CalcLangParser.expression"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	void ExitMul([NotNull] CalcLangParser.MulContext context);
	/// <summary>
	/// Enter a parse tree produced by the <c>Parens</c>
	/// labeled alternative in <see cref="CalcLangParser.expression"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	void EnterParens([NotNull] CalcLangParser.ParensContext context);
	/// <summary>
	/// Exit a parse tree produced by the <c>Parens</c>
	/// labeled alternative in <see cref="CalcLangParser.expression"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	void ExitParens([NotNull] CalcLangParser.ParensContext context);
	/// <summary>
	/// Enter a parse tree produced by the <c>Pow</c>
	/// labeled alternative in <see cref="CalcLangParser.expression"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	void EnterPow([NotNull] CalcLangParser.PowContext context);
	/// <summary>
	/// Exit a parse tree produced by the <c>Pow</c>
	/// labeled alternative in <see cref="CalcLangParser.expression"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	void ExitPow([NotNull] CalcLangParser.PowContext context);
	/// <summary>
	/// Enter a parse tree produced by the <c>Rem</c>
	/// labeled alternative in <see cref="CalcLangParser.expression"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	void EnterRem([NotNull] CalcLangParser.RemContext context);
	/// <summary>
	/// Exit a parse tree produced by the <c>Rem</c>
	/// labeled alternative in <see cref="CalcLangParser.expression"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	void ExitRem([NotNull] CalcLangParser.RemContext context);
	/// <summary>
	/// Enter a parse tree produced by the <c>Int</c>
	/// labeled alternative in <see cref="CalcLangParser.expression"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	void EnterInt([NotNull] CalcLangParser.IntContext context);
	/// <summary>
	/// Exit a parse tree produced by the <c>Int</c>
	/// labeled alternative in <see cref="CalcLangParser.expression"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	void ExitInt([NotNull] CalcLangParser.IntContext context);
	/// <summary>
	/// Enter a parse tree produced by <see cref="CalcLangParser.arglist"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	void EnterArglist([NotNull] CalcLangParser.ArglistContext context);
	/// <summary>
	/// Exit a parse tree produced by <see cref="CalcLangParser.arglist"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	void ExitArglist([NotNull] CalcLangParser.ArglistContext context);
}
} // namespace SharpIrcBot.Plugins.Calc.Language
