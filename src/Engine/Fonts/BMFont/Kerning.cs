// Author:
//       Gabriel Reiser <gabe@reisergames.com>
//
// Copyright (c) 2020-2023 Reiser Games, LLC.
//
// Permission is hereby granted, free of charge, to any person obtaining a copy
// of this software and associated documentation files (the "Software"), to deal
// in the Software without restriction, including without limitation the rights
// to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
// copies of the Software, and to permit persons to whom the Software is
// furnished to do so, subject to the following conditions:
//
// The above copyright notice and this permission notice shall be included in
// all copies or substantial portions of the Software.
//
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
// IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
// FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
// AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
// LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
// OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN
// THE SOFTWARE.

/* AngelCode bitmap font parsing using C#
 * http://www.cyotek.com/blog/angelcode-bitmap-font-parsing-using-csharp
 *
 * Copyright © 2012-2015 Cyotek Ltd.
 *
 * Licensed under the MIT License. See license.txt for the full text.
 */

using System;

namespace Red.Fonts.BMFont
{
    /// <summary>
    ///     Represents the font kerning between two characters.
    /// </summary>
    public struct Kerning : IEquatable<Kerning>
    {
        #region Private Fields

        #endregion Private Fields

        #region Public Constructors

        /// <summary>
        ///     Constructor.
        /// </summary>
        /// <param name="firstCharacter">The first character.</param>
        /// <param name="secondCharacter">The second character.</param>
        /// <param name="amount">
        ///     How much the x position should be adjusted when drawing the second
        ///     character immediately following the first.
        /// </param>
        public Kerning(char firstCharacter, char secondCharacter, int amount)
            : this()
        {
            FirstCharacter = firstCharacter;
            SecondCharacter = secondCharacter;
            Amount = amount;
        }

        #endregion Public Constructors

        #region Public Properties

        /// <summary>
        ///     Gets or sets how much the x position should be adjusted when drawing the second character immediately following the
        ///     first.
        /// </summary>
        /// <value>
        ///     How much the x position should be adjusted when drawing the second character immediately following the first.
        /// </value>
        public int Amount { get; }

        /// <summary>
        ///     Gets or sets the first character.
        /// </summary>
        /// <value>
        ///     The first character.
        /// </value>
        public char FirstCharacter { get; }

        /// <summary>
        ///     Gets or sets the second character.
        /// </summary>
        /// <value>
        ///     The second character.
        /// </value>
        public char SecondCharacter { get; }

        #endregion Public Properties

        #region Public Methods

        /// <summary>
        ///     Check if the object represents kerning between the same two characters.
        /// </summary>
        /// <param name="obj"></param>
        /// <returns>
        ///     Whether or not the object represents kerning between the same two characters.
        /// </returns>
        public override bool Equals(object obj)
        {
            if (obj == null) return false;
            if (obj.GetType() != typeof(Kerning)) return false;

            return Equals((Kerning)obj);
        }

        /// <summary>
        ///     Check if the other kerning is between the same two characters.
        /// </summary>
        /// <param name="other"></param>
        /// <returns>
        ///     Whether or not the other kerning is between the same two characters.
        /// </returns>
        public bool Equals(Kerning other)
        {
            return FirstCharacter == other.FirstCharacter && SecondCharacter == other.SecondCharacter;
        }

        /// <summary>
        ///     Return the hash code of the kerning between the two characters.
        /// </summary>
        /// <returns>
        ///     A unique hash code of the kerning between the two characters.
        /// </returns>
        public override int GetHashCode()
        {
            return (FirstCharacter << 16) | SecondCharacter;
        }

        /// <summary>
        ///     Returns the fully qualified type name of this instance.
        /// </summary>
        /// <returns>
        ///     A <see cref="T:System.String" /> containing a fully qualified type name.
        /// </returns>
        /// <seealso cref="M:System.ValueType.ToString()" />
        public override string ToString()
        {
            return string.Format("{0} to {1} = {2}", FirstCharacter, SecondCharacter, Amount);
        }

        #endregion Public Methods
    }
}