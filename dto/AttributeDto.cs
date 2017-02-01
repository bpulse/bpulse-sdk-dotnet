using System.Collections.Generic;

namespace bpulse_sdk_csharp.dto
{
    /// <summary>
    /// Clase que maneja los atributo tipo long
    /// </summary>
    public class AttributeDto
    {
        private string typeId;
        private List<string> ListAttr_Renamed;

        /// <summary>
        /// contructor de la clase attributo. 
        /// </summary>
        public AttributeDto() : base()
        {
        }

        /// <summary>
        /// sobrecarga del contructor
        /// </summary>
        /// <param name="typeId">id del atributo a manejar</param>
        /// <param name="listAttr">lista de atributos a tomar en cuenta.</param>
        public AttributeDto(string typeId, List<string> listAttr) : base()
        {
            this.typeId = typeId;
            ListAttr_Renamed = listAttr;
        }

        /// <summary>
        /// tipo del atributo
        /// </summary>
        public virtual string TypeId
        {
            get
            {
                return typeId;
            }
            set
            {
                this.typeId = value;
            }
        }

        /// <summary>
        /// lista de atributos
        /// </summary>
        public virtual List<string> ListAttr
        {
            get
            {
                return ListAttr_Renamed;
            }
            set
            {
                ListAttr_Renamed = value;
            }
        }

        /// <summary>
        /// obtencion del codigo hash.
        /// </summary>
        /// <returns>retorna el codigo Hash</returns>
        public override int GetHashCode()
        {
            const int prime = 31;
            int result = 1;
            result = prime * result + ((string.ReferenceEquals(typeId, null)) ? 0 : typeId.GetHashCode());
            return result;
        }

        /// <summary>
        /// sobrecarga del metodo Equals.
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public override bool Equals(object obj)
        {
            if (this == obj)
            {
                return true;
            }
            if (obj == null)
            {
                return false;
            }
            if (this.GetType() != obj.GetType())
            {
                return false;
            }

            AttributeDto other = (AttributeDto)obj;
            if (string.ReferenceEquals(typeId, null))
            {
                if (!string.ReferenceEquals(other.TypeId, null))
                {
                    return false;
                }
            }
            else if (!typeId.Equals(other.TypeId))
            {
                return false;
            }
            return true;
        }
    }
}