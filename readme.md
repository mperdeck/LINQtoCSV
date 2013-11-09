# LINQtoCSV

This library makes it easy to use CSV files with LINQ queries. Its features include:

* Follows the most common rules for CSV files. Correctly handles data fields that contain commas and line breaks.
* In addition to comma, most delimiting characters can be used, including tab for tab delimited fields.
* Can be used with an IEnumarable of an anonymous class - which is often returned by a LINQ query.
* Supports deferred reading.
* Supports processing files with international date and number formats.
* Supports different character encodings if you need them.
* Recognizes a wide variety of date and number formats when reading files.
* Provides fine control of date and number formats when writing files.
* Robust error handling, allowing you to quickly find and fix problems in large input files.

Full documentation is at
http://www.codeproject.com/Articles/25133/LINQ-to-CSV-library

# Contributors welcome

All contributions are welcome, whether those are new features or bug fixes.

Before you invest time in your feature or bug fix, please first raise the issue in the issues list to get feedback about your idea:
https://github.com/mperdeck/LINQtoCSV/issues

For bugs, show how the bug can be reproduced.
For features, show why it would be useful to the wider community.

Introducing a new feature involves more than simply coding the new feature. For every new feature, the following needs to be done:
* Code the feature (obviously);
* Update the documentation in the article.htm file;
* Add unit tests to the LINQtoCSV project, to ensure future code changes don't break your feature.




