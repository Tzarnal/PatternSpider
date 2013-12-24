using System;
using System.Security.Cryptography;


namespace PatternSpider.Utility
{
    public class DiceRoller
    {
        public int RollDice(byte numberSides)
        {
            return InternalRollDice(numberSides);
        }

        public int RollDice(int numberSides)
        {
            if (numberSides < 0)
            {
                throw new ArgumentException("Die size less than 0.");
            } 
            
            if (numberSides > UInt16.MaxValue)
            {
                throw new ArgumentException("No die size larger than " + (UInt16.MaxValue));
            }

            if (numberSides <= 255)
            {
                return InternalRollDice((byte)numberSides);
            }

            return InternalRollDice(numberSides);
        }

        // cheap copy and paste but w/e if it works
        //  http://msdn.microsoft.com/en-us/library/system.security.cryptography.rngcryptoserviceprovider.aspx
        private byte InternalRollDice(byte numberSides)
        {
            if (numberSides <= 0)
                return 1;
            // Create a new instance of the RNGCryptoServiceProvider.
            var rngCsp = new RNGCryptoServiceProvider();
            // Create a byte array to hold the random value.
            var randomNumber = new byte[1];
            do
            {
                // Fill the array with a random value.
                rngCsp.GetBytes(randomNumber);
            }
            while (!IsFairRoll(randomNumber[0], numberSides));
            // Return the random number mod the number
            // of sides.  The possible values are zero-
            // based, so we add one.
            return (byte)((randomNumber[0] % numberSides) + 1);
        }


        private static bool IsFairRoll(byte roll, byte numSides)
        {
            // There are MaxValue / numSides full sets of numbers that can come up
            // in a single byte.  For instance, if we have a 6 sided die, there are
            // 42 full sets of 1-6 that come up.  The 43rd set is incomplete.
            int fullSetsOfValues = Byte.MaxValue / numSides;

            // If the roll is within this range of fair values, then we let it continue.
            // In the 6 sided die case, a roll between 0 and 251 is allowed.  (We use
            // < rather than <= since the = portion allows through an extra 0 value).
            // 252 through 255 would provide an extra 0, 1, 2, 3 so they are not fair
            // to use.
            return roll < numSides * fullSetsOfValues;
        }


        private int InternalRollDice(int numberSides)
        {
            if (numberSides <= 0)
                return 1;
            
            var rngCsp = new RNGCryptoServiceProvider();           
            var randomNumber = new byte[2];

            do
            {                
                rngCsp.GetBytes(randomNumber);
            } while (!IsFairRoll(randomNumber, numberSides));

            return (BitConverter.ToUInt16(randomNumber, 0) % numberSides) + 1;
        }

        private static bool IsFairRoll(byte[] roll, int numSides)
        {
            int fullSetsOfValues = UInt16.MaxValue / numSides;

            return BitConverter.ToUInt16(roll,0) < numSides*fullSetsOfValues;
        }
    }
}
