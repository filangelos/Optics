type Lens<'a,'b> = ( 'a -> 'b ) * ( 'b -> 'a -> 'a )

// abstract get function
let inline get lens = fst lens

// abstract set function
let inline set lens = snd lens

let compose ((get_ab, set_ab): Lens<'a,'b>) ((get_bc, set_bc): Lens<'b,'c>) =
  (get_ab >> get_bc),
  (fun c a -> set_ab (set_bc c (get_ab a)) a) : Lens<'a,'c>

// Type Model
type Model =
  { Brand: string
    ID: string }

  // Model <-> Brand Lens Function
  static member Brand_ =
    ( fun (model: Model) -> model.Brand ), ( fun (brand: string) (model: Model) -> { model with Brand = brand } )

  static member getBrand = get Model.Brand_
  static member setBrand = set Model.Brand_

  // Model <-> ID Lens Fucntion
  static member ID_ =
    ( fun (model: Model) -> model.ID ), ( fun (id: string) (model: Model) -> { model with ID = id } )

    static member getID = get Model.ID_
    static member setID = set Model.ID_


// Type Car
type Car =
  { Color: string
    Model: Model }

  // Car <-> Color Lens Function
  static member Color_ =
    ( fun (car: Car) -> car.Color ), ( fun (color: string) (car: Car) -> { car with Color = color } )

  static member getColor = get Car.Color_
  static member setColor = set Car.Color_

  // Car <-> Model Lens Function
  static member Model_ =
    ( fun (car: Car) -> car.Model ), ( fun (model: Model) (car: Car) -> { car with Model = model } )

  static member getModel = get Car.Model_
  static member setModel = get Car.Model_

  static member getID = get ( compose Car.Model_ Model.ID_ )
  static member setID = set ( compose Car.Model_ Model.ID_ )

  static member getBrand = get ( compose Car.Model_ Model.Brand_ )
  static member setBrand = set ( compose Car.Model_ Model.Brand_ )


// Type Person
type Person = 
  { Age: int
    Car: Car }

  // Person <-> Age Lens Function
  static member Age_ =
    ( fun (person: Person) -> person.Age ), ( fun (age: int) (person: Person) -> { person with Age = age } )

  // Person <-> Car Lens Function
  static member Car_ =
    ( fun (person: Person) -> person.Car ), ( fun (car: Car) (person: Person) -> { person with Car = car } )

  static member getAge = get Person.Age_
  static member setAge = set Person.Age_

  static member getCar = get Person.Car_
  static member setCar = set Person.Car_

  static member getColor = get ( compose Person.Car_ Car.Color_ )
  static member setColor = set ( compose Person.Car_ Car.Color_ )

  static member getModel = get ( compose Person.Car_ Car.Model_ )
  static member setModel = set ( compose Person.Car_ Car.Model_ )

  static member getBrand = get ( compose (Person.getModel, Person.setModel) Model.Brand_ )
  static member setBrand = set ( compose (Person.getModel, Person.setModel) Model.Brand_ )

  static member getID = get ( compose (Person.getModel, Person.setModel) Model.ID_ )
  static member setID = set ( compose (Person.getModel, Person.setModel) Model.ID_ )


// initialise a 'Person'
let tom: Person = 
  { Age = 25
    Car = 
        { Color = "red"
          Model = 
                { Brand = "Audi"
                  ID = "A1" } } }

// Age
let age: int = ( Person.getAge tom )
let tom': Person = ( Person.setAge 60 tom )

// Car
let car: Car = ( Person.getCar tom )

let tCar: Car = { Color = "Blue" ; Model = { Brand = "MB" ; ID = "A-Class" } }
let tom'': Person = ( Person.setCar tCar tom' )

let id: string = ( Person.getID tom'' )

// operator shortcut
let ( ^= ) = Person.setID

let tom''': Person = ( ^= ) "C-Class" tom''

printfn "tom: %A" tom
printfn "age: %A" age
printfn "tom': %A" tom'
printfn "car': %A" car
printfn "tom'': %A" tom''
printfn "id: %A" id
printfn "tom''': %A" tom'''