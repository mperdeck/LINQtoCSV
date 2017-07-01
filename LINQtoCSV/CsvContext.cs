using System;
using System.Collections.Generic;
using System.IO;

namespace LINQtoCSV
{
    /// <summary>
    /// A context provider for reading values from CSV files.
    /// </summary>
    public class CsvContext
    {
        #region Read

        /// <summary>
        /// Reads the comma separated values from a stream or file and returns the data into an
        /// <see cref="IEnumerable{T}"/> that can be used for LINQ queries.
        /// </summary>
        /// <typeparam name="T">
        /// The records in the returned <see cref="IEnumerable{T}"/> will be of this type.
        /// </typeparam>
        /// <param name="stream">
        /// <para>All data is read from this stream, unless fileName is not null.</para>
        /// <para>
        /// This is a <see cref="System.IO.StreamReader"/> rather then a <see cref="System.IO.TextReader"/>,
        /// because we need to be able to seek back to the start of the
        /// stream, and you can't do that with a <see cref="System.IO.TextReader"/> (or <see cref="System.IO.StreamReader"/>).
        /// </para>
        /// </param>
        /// <returns>
        /// Values read from the stream or file.
        /// </returns>
        /// <remarks>
        /// The stream or file will not be closed after the last line has been processed.
        /// Because the library implements deferred reading (using Yield Return), please be careful
        /// about closing the stream reader.
        /// </remarks>

        public IEnumerable<T> Read<T>(StreamReader stream) where T : class, new()
        {
            return Read<T>(stream, new CsvFileDescription());
        }

        /// <summary>
        /// Reads the comma separated values from a stream or file and returns the data into an
        /// <see cref="IEnumerable{T}"/> that can be used for LINQ queries.
        /// </summary>
        /// <typeparam name="T">
        /// The records in the returned <see cref="IEnumerable{T}"/> will be of this type.
        /// </typeparam>
        /// <param name="stream">
        /// <para>All data is read from this stream, unless fileName is not null.</para>
        /// <para>
        /// This is a <see cref="System.IO.StreamReader"/> rather then a <see cref="System.IO.TextReader"/>,
        /// because we need to be able to seek back to the start of the
        /// stream, and you can't do that with a <see cref="System.IO.TextReader"/> (or <see cref="System.IO.StreamReader"/>).
        /// </para>
        /// </param>
        /// <param name="fileDescription">
        /// Additional information how the input file is to be interpreted, such as the culture of the input dates.
        /// </param>
        /// <returns>
        /// Values read from the stream or file.
        /// </returns>
        /// <remarks>
        /// The stream or file will not be closed after the last line has been processed.
        /// Because the library implements deferred reading (using Yield Return), please be careful
        /// about closing the stream reader.
        /// </remarks>
        public IEnumerable<T> Read<T>(
                    StreamReader stream,
                    CsvFileDescription fileDescription) where T : class, new()
        {
            // If T implements IDataRow, then we're reading raw data rows
            bool readingRawDataRows = typeof(IDataRow).IsAssignableFrom(typeof(T));

            // The constructor for FieldMapper_Reading will throw an exception if there is something
            // wrong with type T. So invoke that constructor before you open the file, because if there
            // is an exception, the file will not be closed.
            //
            // If T implements IDataRow, there is no need for a FieldMapper, because in that case we're returning
            // raw data rows.
            FieldMapper_Reading<T> fm = null;

            if (!readingRawDataRows)
            {
                fm = new FieldMapper_Reading<T>(fileDescription, false);
            }

            // -------
            // Each time the IEnumerable<T> that is returned from this method is
            // accessed in a foreach, ReadData is called again (not the original Read overload!)
            //
            // So, open the file here, or rewind the stream.

            // Rewind the stream

            if ((stream == null) || (!stream.BaseStream.CanSeek))
            {
                throw new BadStreamException();
            }

            stream.BaseStream.Seek(0, SeekOrigin.Begin);

            // ----------

            CsvStream cs = new CsvStream(stream, null, fileDescription.SeparatorChar, fileDescription.IgnoreTrailingSeparatorChar);

            // If we're reading raw data rows, instantiate a T so we return objects
            // of the type specified by the caller.
            // Otherwise, instantiate a DataRow, which also implements IDataRow.
            IDataRow row = null;
            if (readingRawDataRows)
            {
                row = new T() as IDataRow;
            }
            else
            {
                row = new DataRow();
            }

            AggregatedException ae =
                new AggregatedException(typeof(T).ToString(), fileDescription.MaximumNbrExceptions);

            try
            {
                List<int> charLengths = null;
                if (!readingRawDataRows)
                {
                    charLengths = fm.GetCharLengths();
                }

                bool firstRow = true;
                while (cs.ReadRow(row, charLengths))
                {
                    // Skip empty lines.
                    // Important. If there is a newline at the end of the last data line, the code
                    // thinks there is an empty line after that last data line.
                    if ((row.Count == 1) &&
                        ((row[0].Value == null) ||
                         (string.IsNullOrEmpty(row[0].Value.Trim()))))
                    {
                        continue;
                    }

                    if (firstRow && fileDescription.FirstLineHasColumnNames)
                    {
                        if (!readingRawDataRows) { fm.ReadNames(row); }
                    }
                    else
                    {
                        T obj = default(T);
                        try
                        {
                            if (readingRawDataRows)
                            {
                                obj = row as T;
                            }
                            else
                            {
                                obj = fm.ReadObject(row, ae);
                            }
                        }
                        catch (AggregatedException ae2)
                        {
                            // Seeing that the AggregatedException was thrown, maximum number of exceptions
                            // must have been reached, so re-throw.
                            // Catch here, so you don't add an AggregatedException to an AggregatedException
                            throw ae2;
                        }
                        catch (Exception e)
                        {
                            // Store the exception in the AggregatedException "ae".
                            // That way, if a file has many errors leading to exceptions,
                            // you get them all in one go, packaged in a single aggregated exception.
                            ae.AddException(e);
                        }

                        yield return obj;
                    }
                    firstRow = false;
                }
            }
            finally
            {
                // If any exceptions were raised while reading the data from the file,
                // they will have been stored in the AggregatedException "ae".
                // In that case, time to throw "ae".
                ae.ThrowIfExceptionsStored();
            }
        }

        #endregion Read

        /// ///////////////////////////////////////////////////////////////////////
        /// Write
        ///
        public void Write<T>(
            IEnumerable<T> values,
            TextWriter stream)
        {
            Write(values, stream, new CsvFileDescription());
        }

        public void Write<T>(
            IEnumerable<T> values,
            TextWriter stream,
            CsvFileDescription fileDescription)
        {
            FieldMapper<T> fm = new FieldMapper<T>(fileDescription, true);
            CsvStream cs = new CsvStream(null, stream, fileDescription.SeparatorChar, fileDescription.IgnoreTrailingSeparatorChar);

            List<string> row = new List<string>();

            // If first line has to carry the field names, write the field names now.
            if (fileDescription.FirstLineHasColumnNames)
            {
                fm.WriteNames(row);
                cs.WriteRow(row, fileDescription.QuoteAllFields);
            }

            // -----

            foreach (T obj in values)
            {
                // Convert object to row
                fm.WriteObject(obj, row);
                cs.WriteRow(row, fileDescription.QuoteAllFields);
            }
        }
    }
}