# Dalio, Brian A.
# dalioba
# 2020-03-02
#---------#---------#---------#---------#---------#--------#
class Roman() :
  c_RomanNumerals = (
    ( 1000000, 'μ', 'μ' ),
    (  500000, 'δ', 'δ' ),
    (  100000, 'χ', 'χ' ),
    (   50000, 'λ', 'λ' ),
    (   10000, 'ξ', 'ξ' ),
    (    5000, 'β', 'β' ),

    ( 1000, 'M',  'M' ),
    (  900, 'CM', 'ϡ' ),
    (  500, 'D',  'D' ),
    (  400, 'CD', 'P' ),
    (  100, 'C',  'C' ),
    (   90, 'XC', 'N' ),
    (   50, 'L',  'L' ),
    (   40, 'XL', 'F' ),
    (   10, 'X',  'X' ),
    (    9, 'IX', '9' ),
    (    5, 'V',  'V' ),
    (    4, 'IIII', '4' ),
    (    1, 'I',  'I' ) )

  @classmethod
  def toRoman( cls, number ) :
    if not isinstance( number, int ) :
      raise ValueError( f'Roman number must be integer, not {type(number)}.' )

    if number <= 0 :
      raise ValueError( f'Roman number must be > 0, not {number}.' )

    roman = ''
    for unit, code, _ in Roman.c_RomanNumerals :
      roman += code*int(number/unit)
      number %= unit

    return roman

  @classmethod
  def fromRoman( cls, roman ) :
    if not isinstance( roman, str ) :
      raise ValueError( f'Roman number must be string, not {type(roman)}.' )

    original = roman

    for _, code, single in Roman.c_RomanNumerals :
      roman = roman.replace( code, single )

    number = 0
    for unit, _, single in Roman.c_RomanNumerals :
      number += roman.count( single )*unit

    if ( number > 0 ) and ( original == Roman.toRoman( number ) ) :
      return number

    else :
      return None

#---------#---------#---------#---------#---------#--------#

