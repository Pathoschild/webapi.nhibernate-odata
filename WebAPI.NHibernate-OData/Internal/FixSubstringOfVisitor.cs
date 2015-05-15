using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;

namespace Pathoschild.WebApi.NhibernateOdata.Internal
{
    /// <summary>Intercepts queries before they're parsed by NHibernate to rewrite unsupported nullable-boolean conditions into supported boolean conditions.</summary>
    public class FixSubstringOfVisitor : ExpressionVisitor
    {
        /// <summary>Whether the visitor is visiting a nested node.</summary>
        /// <remarks>This is used to recognize the top-level node for logging.</remarks>
        private bool _isRecursing;

        /// <summary>The nodes to rewrite.</summary>
        protected readonly HashSet<Expression> NodeRewriteList = new HashSet<Expression>();

        /// <summary>Dispatches the expression to one of the more specialized visit methods in this class.</summary>
        /// <returns>The modified expression, if it or any subexpression was modified; otherwise, returns the original expression.</returns>
        /// <param name="node">The expression to visit.</param>
        public override Expression Visit(Expression node)
        {
            // top node
            if (!this._isRecursing)
            {
                this._isRecursing = true;
                return base.Visit(node);
            }

            return base.Visit(node);
        }
    }
}
