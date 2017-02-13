using System.Collections.Generic;

namespace bpulse_sdk_csharp.dto
{
    /// <summary>
    ///      Clase que maneja los atributo tipo long
    /// </summary>
    public class AttributeDto
    {
        #region Private Fields

        private List<string> _listAttrRenamed;
        private string _typeId;

        #endregion Private Fields

        #region Public Constructors

        /// <summary>
        ///      contructor de la clase attributo.
        /// </summary>
        public AttributeDto()
        {
        }

        /// <summary>
        ///      sobrecarga del contructor
        /// </summary>
        /// <param name="typeId">id del atributo a manejar</param>
        /// <param name="listAttr">lista de atributos a tomar en cuenta.</param>
        public AttributeDto(string typeId, List<string> listAttr)
        {
            _typeId = typeId;
            _listAttrRenamed = listAttr;
        }

        #endregion Public Constructors

        #region Public Properties

        /// <summary>
        ///      lista de atributos
        /// </summary>
        public virtual List<string> ListAttr
        {
            get { return _listAttrRenamed; }
            set { _listAttrRenamed = value; }
        }

        /// <summary>
        ///      tipo del atributo
        /// </summary>
        public virtual string TypeId
        {
            get { return _typeId; }
            set { _typeId = value; }
        }

        #endregion Public Properties

        #region Public Methods

        /// <summary>
        ///      sobrecarga del metodo Equals.
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public override bool Equals(object obj)
        {
            if (this == obj)
                return true;
            if (obj == null)
                return false;
            if (GetType() != obj.GetType())
                return false;

            var other = (AttributeDto)obj;
            if (ReferenceEquals(_typeId, null))
            {
                if (!ReferenceEquals(other.TypeId, null))
                    return false;
            }
            else if (!_typeId.Equals(other.TypeId))
            {
                return false;
            }
            return true;
        }

        /// <summary>
        ///      obtencion del codigo hash.
        /// </summary>
        /// <returns>retorna el codigo Hash</returns>
        public override int GetHashCode()
        {
            const int prime = 31;
            var result = 1;
            result = prime * result + (ReferenceEquals(_typeId, null) ? 0 : _typeId.GetHashCode());
            return result;
        }

        #endregion Public Methods
    }
}