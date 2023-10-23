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

using System.IO;

namespace Reactor.Fonts.BMFont
{
  /// <summary>
  ///     Represents a texture page.
  /// </summary>
  public struct Page
    {
        #region Private Fields

        #endregion Private Fields

        #region Public Constructors

        /// <summary>
        ///     Creates a texture page using the specified ID and source file name.
        /// </summary>
        /// <param name="id">The identifier.</param>
        /// <param name="fileName">Filename of the texture image.</param>
        public Page(int id, string fileName)
            : this()
        {
            FileName = fileName;
            Id = id;
        }

        #endregion Public Constructors

        #region Public Properties

        /// <summary>
        ///     Gets or sets the filename of the source texture image.
        /// </summary>
        /// <value>
        ///     The name of the file containing the source texture image.
        /// </value>
        public string FileName { get; set; }

        /// <summary>
        ///     Gets or sets the page identifier.
        /// </summary>
        /// <value>
        ///     The page identifier.
        /// </value>
        public int Id { get; set; }

        #endregion Public Properties

        #region Public Methods

        /// <summary>
        ///     Returns the fully qualified type name of this instance.
        /// </summary>
        /// <returns>
        ///     A <see cref="T:System.String" /> containing a fully qualified type name.
        /// </returns>
        /// <seealso cref="M:System.ValueType.ToString()" />
        public override string ToString()
        {
            return string.Format("{0} ({1})", Id, Path.GetFileName(FileName));
        }

        #endregion Public Methods
    }
}