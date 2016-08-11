#region License

// A simplistic Behavior Tree implementation in C#
// Copyright (C) 2010-2011 ApocDev apocdev@gmail.com
// 
// This file is part of TreeSharp
// 
// This program is free software: you can redistribute it and/or modify
// it under the terms of the GNU Lesser General Public License as published by
// the Free Software Foundation, either version 3 of the License, or
// (at your option) any later version.
// 
// This program is distributed in the hope that it will be useful,
// but WITHOUT ANY WARRANTY; without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
// GNU General Public License for more details.
// 
// You should have received a copy of the GNU General Public License
// along with this program.  If not, see <http://www.gnu.org/licenses/>.

// TODO: THIS WAS A NEW FILE -- MODIFY THIS HEADER
#endregion

using System;
using System.Collections.Generic;

namespace TreeSharpPlus
{
    /// <summary>
    /// Loops will continue executing their child indefinitely unless that child reports
    /// failure. If the child reports success, the proceed loop will restart it from the beginning.
    /// </summary>
    public class LoopAssert : Node
    {
        /// <summary>
        ///     The number of iterations to run (-1 is infinite)
        /// </summary>
        public int Iterations { get; set; }

        protected Func<bool> func_assert = null;

        public LoopAssert(Func<bool> assertion)

        {
            this.func_assert = assertion;
        }


        public override IEnumerable<RunStatus> Execute()
        {
            // Keep track of the running iterations

            while(true)
            {
 

                if (this.func_assert != null)
                {
                    bool func_result = this.func_assert.Invoke();
                    if (func_result == true)
                    {
                        yield return RunStatus.Success;

                        yield break;
                    }
                }
                else
                {
                    throw new ApplicationException(this + ": No method given");
                }

                // Take one tick to prevent infinite loops
                yield return RunStatus.Running;
            }
        }
    }
}