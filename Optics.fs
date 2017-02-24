[<AutoOpen>]
module Optics =

  type Lens<'a,'b> = ( 'a -> 'b ) * ( 'b -> 'a -> 'a )

  // abstract get function
  let inline get lens = fst lens

  // abstract set function
  let inline set lens = snd lens

  // Compose
  let compose ((get_ab, set_ab): Lens<'a,'b>) ((get_bc, set_bc): Lens<'b,'c>) =
    (get_ab >> get_bc),
    (fun c a -> set_ab (set_bc c (get_ab a)) a) : Lens<'a,'c>