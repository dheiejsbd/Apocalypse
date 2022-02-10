using System;

namespace HQFPSTemplate
{
    public class IdGenerator
    {
        private static Random m_Random = new Random();


        /// <summary>
        /// Returns a string id intended for use in small or medium collections. <br></br>
        /// If you need a more global id which is almost guaranteed to be unique use System.Guid.
        /// </summary>
        public static string GenerateStringId()
        {
            byte[] buffer = GetRandom(4);
            string id = Convert.ToBase64String(buffer);

            return id.Remove(id.Length - 3);
        }

        /// <summary>
        /// Returns an integer id intended for use in small or medium collections. <br></br>
        /// If you need a more global id which is almost guaranteed to be unique use System.Guid. <br></br>
        /// The id will have 7 digits max so it can be stored as a float, if needed.
        /// </summary>
        public static int GenerateIntegerId()
        {
            return m_Random.Next(-9999999, 9999999);
        }

        private static byte[] GetRandom(int size)
        {
            byte[] bytes = new byte[size];
            m_Random.NextBytes(bytes);

            return bytes;
        }
    }
}