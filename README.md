# Optics in F\# #

## Background

F# Records resemble C type structs; they have named 'properties' and can have `member` functions. <br>
Due to the functional nature of the language *mutation* of their associated properties is not compact and inarguably, not recommended. <br>
In object oriented paradigm, in order to encapsulate the low level implementation of each object, (usually) each property has a `get` and a `set` function that are responsible for reading and modifying the property, respectively. <br>
For a general object `'a` with property `'b`, these functions can be modelled as:
```
get: 'a -> 'b
set: 'b -> 'a -> unit
```
While the `get` functions is `acceptable` in functional programming, the `set` function is not, since it generates a side-effect (muatation). <br>
On the other hand, `set` could be modified such that it returns a new `'b` with the updated property, then the signature would become:
```
set: 'b -> 'a -> 'a
```
Great!! This is exactly what we need in FP, a immutable type, whose properties can be updated (`get`) in a controllable and easy to reason about manner. <br>
Consider the example below, with the Record `Person`.
```F#
// sandbox type Model
type Model =
  { Brand: string
    ID: string }

// sandbox type Car
type Car =
  { Color: string
    Model: Model }

// sandbox type Person
type Person = 
  { Age: int
    Car: Car }

// initialise a 'Person'
let tom: Person = 
  { Age = 25
    Car = 
        { Color = "red"
          Model = 
                { Brand = "Audi"
                  ID = "A1" } } }

// Get the car model id
let model: string = tom.Car.Model.ID

// mutable setter
// It will throw an ERROR since the field is immutable !!!
// tom.Car.Model.Brand <- "BMW"

// Immutable Setter
let tom': Person =
  { tom with Car =
                 { tom.Car with Model = 
                                      { tom.Car.Model with ID = "A3" } } }
```
As far as the `getter` is concerned, the syntax is quite OOP-ish, but intuitive and handy. <br>
On the other hand, the `immutable setter` is a total disaster, too verbose, error-prone and it can quickly get out of hand. <br>
Do not give up on functional programming yet, though.

## Example

We showed previously that regardless of the complexity of the data structure, the `get` and `set` functions can be reduced to a basic top-level-generator, which can be used for as a building block for any data structure. <br> 
Let's put this to code. We will first work on the `Person <-> Car` relation and similarly we will extend this later to `Person <-> Car <-> Model <-> ID`
```
// 'a = Person
// `b = Car

// Person <-> Car getter
let getCar (person: Person) : Car = person.Car

// get example
let car: Car = getCar tom' // { Color = "red" ; Model = { Brand = "Audi" ; ID = "A3" } }

// Person <-> Car setter
let setCar (car: Car) (person: Person) : Person = 
  { person with Car = car }

// set example
let bmwM3W: Car = { Color = "white" ; Model = { Brand = "BMW" ; ID = "M3" } }
let tom'': Person = setCar bmwM3W tom' // { Age = 25 ; Car = { Color = "white" ; Model = { Brand = "BMW" ; ID = "M3" } } }
```
So we managed to abstract the details using the `set` and `get` functions, promoting modularity and clarity. Let's move one step further and deal with `Car <-> Model` relation and then "glue" the two together.
```F#
// 'a = Car
// `b = Model

// Car <-> Model getter
let getModel (car: Car) : Model = car.Model

// get example
let model': Model = tom' |> getCar |> getModel // getter composition

// Car <-> Model setter
let setModel (model: Model) (car: Car) : Car = 
  { car with Model = model }

// set example
let fiatPundoB: Car = { Color = "Black" ; Model = { Brand = "Aston Martin" ; ID = "DB9" } }
let amDB9B: Car = setModel (getModel bmwM3W) fiatPundoB // setter composition
```
It may seem a bit clamsy, but this is because in these examples I create random varialbes to illustrate the functionality of the `set`, `get` and all the place is filled with initialisations. <br>
In practice the data will be generated in the beginning of the execution and then we will need to access and modify them. <br>
Additionally, we note that we can create compositions of `get` and `set` functions, such as:
```
get_ab : 'a -> 'b
get_bc : 'b -> 'c
get_comp: 'a -> 'c === get_ab >> get_bc

set_ab : 'b -> 'a -> 'a
set_bc : 'c -> 'b -> 'b
set_comp: 'c -> 'a -> 'a === set_ab ( set_bc 'c (get_ab a) ) 'a
```
We have come a long way from `mutable` to verbose `mutable` to `compact` `setters` and `getters` but we can do better!!! <br>
We will define a new abstract type `Lens<'a, 'b>` that will be a tuple of the `get` and the `set` function.
```
type Lens<'a,'b> = ( 'a -> 'b ) * ( 'b -> 'a -> 'a )

// abstract get function
let inline get lens = fst lens

// abstract set function
let inline set lens = snd lens
```
> note that the `inline` keyword is used in order to keep the definition generic
> and not let the compiler infere the type the first time we call any of the functions. <br>
> Use this [reference](http://blog.2mas.xyz/constraints-in-fsharp/) if not clear.


Now we can define a `Lens` for each property of Record and just by defining 
a new top-level-function `let fooGetter = Optics.get <Record>.<Lens>` and `let fooSetter = Optics.set <Record>.<Lens>`
we have our functionally clean `get` and `set` functions ready. <br>

## Test

Run `Test.fsx` that gives the complete implementation of the sandbox example.

## Installation

Import `Optics.fs` file to your project and reference all of each functions as `Optics.<name of function>`.

## To Do

Implement `Prism` for `Option` return types. ( ~ by 28.02.17 )

## References

1. [Haskell Docs](https://hackage.haskell.org/package/lens)
2. [A Little Lens Starter Tutorial](https://www.schoolofhaskell.com/school/to-infinity-and-beyond/pick-of-the-week/a-little-lens-starter-tutorial)
3. [Lenses in F#](http://bugsquash.blogspot.co.uk/2011/11/lenses-in-f.html)
4. [Inline Functions](http://blog.2mas.xyz/constraints-in-fsharp/)
5. [MSDN Inline Functions](https://docs.microsoft.com/en-us/dotnet/articles/fsharp/language-reference/functions/inline-functions)
