# Spice
 A programming language for 'Golfing' in an assembly-like environment. 

 ## TL;DR
 - Spice is an interpreted assembly-like language with a handful of operators:
    - `ADD`, `SUB`, `MUL`, `DIV`, `MOD`, `PUT`, `GET`, `SWI`, `BRK`, `ALS`, `OUT`, `LOD`, `SIN`, `COS`, `TAN`, `POW`, `REA`, `CLR`, `LEN`, `NUL`
 - Spice programs are split into two sections; `declaration` and `instruction` split with an `@` character.
 - All values are dynamically sized `double` arrays with the exception of string literals, `"Some string"`, which may be used in OUT statements, but not stored
 - The end of instructions is denoted by a user-defined character - the first character of the program

 An example program is given below:

 ``` Spice
 ;input;result;@
 OUT "Input a number:";
 REA input;
 OUT "Number is" input;
 MOD 10 input result;
 OUT input "mod 10 is" result;
 ALS OUT SAY;
 SAY "Now SAY is a valid operator";
 LOD .\module.spice input;
 BRK 1 0 result;
 ```

 ## Interpreter

 Invoking a Spice program takes the following format:

 `spice.exe .\prog.spice outputLevel`

 Where `spice.exe` is the interpreter, `.\prog.spice` is a valid Spice source file and outputLevel is a number denoting the verbosity of the program.

 Output levels are:
 - PROGRAM = 0 (Least verbose, program output only)
 - ERROR = 1
 - DEBUG = 2
 - INFO = 3 (Most verbose)

 ## Declaration
 In the above program, `;` is declared as the program delimiter (note newline characters are still accepted with a non-newline separator). Following the delimiter declaration is the declaration of all variables used in the program. In a module the order of varable declaration is important, as values passed to a module are loaded into each variable in declaration order.

 Instructions may only be used after declaration is complete, denoted by the '`@`' character.


 ## Operators

 In any case where a single value is expected but an array is passed, the value resolves to the 0th element (in both setting and getting values). Empty arrays have an implicit value of `0`. For example, `ADD a b c` where `a`, `b` and `c` are all arrays is equivalent to (not valid Spice code): `ADD a[0] b[0] c[0]`.

 ### `ADD`, `SUB`, `MUL`, `DIV`, `MOD`, `POW`
 Add, subtract, multiply, divide, modulus, power

 Format:
  
 `<OP> x y z`
  
 Explanation:

 The operator is applied to `x` and `y` and the result stored in `z`. `x` and `y` may be value literals or variable names. `z` must be a variable name.

 Eg. 
 - `ADD 3 5 result` is the equivalent to `result = 3 + 5`
 - `POW 2 3 result` is the equivalent to `result = 2^3`
  
 ### `SIN`, `COS`, `TAN`
 Sin, Cos, Tan

 Format:
 
 `<OP> x y`

 Explanation:

 The operator is applied to `x` and the result stored in `y`. `x` may be a value literal or variable name. `y` must be a variable name.

  Eg. 
  - `SIN 5 result` is the equivalent to `result = Sin(5)`


 ### `PUT`
 Insert into array

 Format:
  
 `PUT x y z`
  
 Explanation:

 The value of `z` is inserted into the array `y` at position `x`. `x` is floored, to become the index. `z` may be a value literal or a variable. A list must be initalised with a value to be able to insert into it - eg `ADD 50 0 y` will add `50` at index `0`. The value of `x` must not be outside the range 0 to array length - 1.

 Eg. 
 - `PUT 1.2 array 6` is the equivalent to `array[1] = 6`
 - `PUT 5.7 array 10.8` is the equivalent to `array[5] = 10.8`


 ### `GET`
 Retrieve from array

 Format:
  
 `GET x y z`
  
 Explanation:

 The value at index `x` of the array, `y`, is stored in the variable `z`. `x` is floored, to become the index. The value of `x` must not be outside the range 0 to array length - 1.

 Eg. 
 - `GET 1.2 array var` is the equivalent to `var = array[1]`
 - `GET 5 array var` is the equivalent to `var = array[5]`


 ### `SWI`
 Format:
  
 `SWI x y z`
  
 Explanation:

 If `x` is less than `y`, the program counter is set to the zero-indexed operation number of `z`. The operation count begins from after the '`@`' program split. `x`, `y` and `z` may all be value literals or variable names.

 Eg. 
 - `SWI 1.2 5 6` results in the program jumping to line 6 as 1.2 is less than 5
 - `SWI 2 0 0` results in the program continuing as if SWI had been a NUL statement

 ### `BRK`
 Format:
  
 `BRK x y z`
  
 Explanation:

 If `x` is less than `y`, the program ends. Otherwise, `z` is set to the absolute difference between `x` and `y`. `x` and `y` may both be value literals or variable names, `z` must be a variable name.

 Eg. 
 - `BRK 1.2 5 var` results in the program terminating as 1.2 is less than 5
 - `BRK 3 1 var` results in the program continuing, with var having the value 2


 ### `ALS`
 Alias an operator

 Format:
  
 `ALS x y`
  
 Explanation:

 The OP `x` (which must be a valid OP keyword, eg, `ADD`, `OUT`, `PUT` etc) is aliased to `y`.

 Eg. 
 - `ALS GET RETRIEVE` results in `RETRIEVE` being an alias to `GET`
 - `ALS BRK END_IF_LSS_THN` results in `END_IF_LSS_THN` being an alias to `BRK`


 ### `OUT`
 Output to the console

 Format:
  
 `OUT a b c d <...> x y z`
  
 Explanation:

 The value literals or variable values of any following argument is printed to the console. The arguments may be a double value literal, a variable name or a `"string literal"`. A string literal may span multiple lines, 

 Eg. 
 - `OUT var "hello" 200.2` results in the value of `var`, the string `"hello"` and `200.2` being printed to the console.
 - `ALS BRK END_IF_LSS_THN` results in `END_IF_LSS_THN` being an alias to `BRK`

 ### `LOD`
 Invoke a Spice source file with the given argument(s)

 Format:
  
 `LOD source.path arguments z`
  
 Explanation:

 The 'module' in the source file at `source.path` is loaded into its own context. The declared values for the module are set as they are initalised to the value(s) of `arguments`. To instead set the value of ony the first variable to the value or arguments the syntax `^arguments` should be used.
 
 Modules must declare the variable `return` which is passed back to the calling program and assigned to the passed variable `z`. Module invocations may be nested indefinitely.

 Eg. 
 Module source (module.spice):

 ``` Spice
 ;input1;input2;return@
 OUT input1 input2;
 ADD 100 0 return;
 ```

 This module is called in the following context:

 ``` Spice
 ...
 PUT 0 array 50
 PUT 1 array 100
 LOD .\module.spice array result
 ```

 This results in the values `50` and `100` being output to the console. `result` is equal to `100` when control returns to the main program.

 If instead `LOD .\module.spice array result` was used, the module would have `input1` initalised to `[50, 100]` and would output "`[50, 100] [0]`".

 ### `REA`
 Read value(s) from the console

 Format:
  
 `REA input`
  
 Explanation:

 The program outputs a '`>`' character and waits for the user to input some value(s). Values must be double literals or hex (denoted by `#` eg `#AA0BB9`) separated by '`, `' (note the space character).

 Eg. 
 - `REA value` where user enters `1200` results in `value` being set to `1200`
 - `REA value` where user enters `#0x4007B425F202107B` results in `value` being set to `2.962963`

 ### `CLR`
 Reset a variable

 Format:
  
 `CLR x`
  
 Explanation:

 The value of `x` is cleared. `x` is set to an empty array/an implicit value of `0`.

 Eg. 
 - `CLR value` - `value` is equal to `[]`/`0`
 
 ### `LEN`
 Get the length of a variable

 Format:
  
 `LEN x y`
  
 Explanation:

 The length of `x` is stored in `y`.

 Eg. 
 - `LEN arr count` - `count` is equal to the length of arr

### `NUL`
A no-op

Format:

`NUL a b c <...> x y z`

Explanation:

A no-op, nothing happens. Any arguments are not resolved, and so it may be used as a method for adding comments. A NUL op does count towards line/op count for `SWI` statements.

Eg.
- `NUL This is a comment 100! woo-hoo!` - Nothing happens.