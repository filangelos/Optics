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

// 'a = Person
// `b = Car

// Person <-> Car getter
let getCar (person: Person) : Car = person.Car

// get example
let car: Car = getCar tom' // { Color = "red" ; Model = { Brand = "Audi" ; ID = "A3" } }

// Person <-> Model setter
let setCar (car: Car) (person: Person) : Person = 
  { person with Car = car }

let bmwM3W: Car = { Color = "white" ; Model = { Brand = "BMW" ; ID = "M3" } }

// set example
let tom'': Person = setCar bmwM3W tom' // { Age = 25 ; Car = { Color = "white" ; Model = { Brand = "BMW" ; ID = "M3" } } }

// 'a = Car
// `b = Model

// Car <-> Model getter
let getModel (car: Car) : Model = car.Model

// get example
let model': Model = tom' |> getCar |> getModel

// Car <-> Model setter
let setModel (model: Model) (car: Car) : Car = 
  { car with Model = model }

// set example
let fiatPundoB: Car = { Color = "Black" ; Model = { Brand = "Aston Martin" ; ID = "DB9" } }
let amDB9B: Car = setModel (getModel bmwM3W) fiatPundoB // nice level-up
