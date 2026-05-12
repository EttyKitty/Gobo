<h1 align="center">
  Gobo: GML Formatter (Etty's fork⭐)
</h1>

<h4 align="center">
  Try online version of the formatter here: https://ettykitty.github.io/Gobo/
</h4>

## What is this?

Gobo is an opinionated formatter for GameMaker Language. It enforces a consistent style by parsing and re-printing your code with its own rules.

By using Gobo, you agree to cede control over the nitty-gritty details of formatting. In return, Gobo gives you speed, determinism, and smaller git diffs. End style debates with your team and save mental energy for what's important!

Gobo provides a few basic formatting options ~~and has no plans to add more. It follows the [Option Philosophy](https://prettier.io/docs/en/option-philosophy.html) of Prettier.~~
- `Max Line Width`
- `Use Tabs or Spaces` (custom size)
- `Flat Expressions` (expressions ignore `Max Line Width`)

## ⭐What is different in this fork? It's slightly less opinionated⭐

Default settings are my take on *opinionated* philosophy, and *are* opinionated.  
*Giving* options I don't believe ever should be. Unless a pain to maintain, which most are not.

### New options
- `Vertical Structs` option
- `Vertical Arrays` option (1-length arrays are exempt)
- (Both of the above work only during variable initialization/declaration, and won't format vars in statements/expressions)
- `Limit Width` on/off option (instead of setting `Max Line Width` to 999)
- `\n After Blocks` option to add a blank line between a block statement and any subsequent statement at the same scope level (.NET [IDE2003](https://learn.microsoft.com/en-us/dotnet/fundamentals/code-analysis/style-rules/ide2003) style)

### New opinionated logic
- Array accessors are chained separately like in JS (`array[0, 2]` > `array[0][2]`; not an option because I hate these. May add a toggle later)

## Example

```js
// Input
x = a and b or c  a=0xFG=1 var var var i := 0 s={d: 5, f: 9}
do begin
;;;;show_debug_message(i)
;;;;i++
end until not i < 10 return

call()

// Output
x = a && b || c;
a = 0xF;
G = 1;
var i = 0;
s = {
    d: 5,
    f: 9,
};
do {
    show_debug_message(i);
    i++;
} until (!i < 10)

return call();
```


## Limitations

Gobo cannot parse code that relies on macro expansion to be valid. Any standalone expression will be formatted with a semicolon, even if the expression is a macro.
```js
THESE_MACROS;
ARE.VALID;
BECAUSE_THEY_ARE_EXPRESSIONS()
```

## How does it work?

Gobo is written in C# and compiles to a self-contained binary using Native AOT in .NET 8.

It uses a custom GML parser to read your code and ensure that formatted code is equivalent to the original. The parser is designed to only accept valid GML (with a few exceptions) to ensure correctness. There is no officially-documented format for GML's syntax tree, so Gobo uses a format similar to JavaScript parsers. 

Your code is converted into an intermediate "Doc" format to make decisions about wrapping lines and printing comments. The doc printing algorithm is taken from [CSharpier](https://github.com/belav/csharpier), which is itself adapted from Prettier.

