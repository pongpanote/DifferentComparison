# DifferentComparison
A REST service that compare two Base64 encoded binary data.

There are two setter http endpoints \<host\>/v1/diff/\<ID\>/left and \<host\>/v1/diff/\<ID\>/right.
Each endpoint accepts JSON containing base64 encoded binary data.

Only the data on both aforementioned endpoints were set, a difference can be calculated.
Using \<ID\> as a key, the results shall be available on a third end point \<host\>/v1/diff/\<ID\>.

The results shall provide the following info in JSON format:
- If equal return that
- If not of equal in size just return that
- If of same size provide insight in where the diff are, actual diffs are not needed.
