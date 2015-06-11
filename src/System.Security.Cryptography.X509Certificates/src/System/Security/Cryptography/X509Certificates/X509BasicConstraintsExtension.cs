// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.IO;
using System.Text;
using System.Diagnostics;
using System.Globalization;
using System.Runtime.InteropServices;

using Internal.Cryptography;
using Internal.Cryptography.Pal;

namespace System.Security.Cryptography.X509Certificates
{
    public sealed class X509BasicConstraintsExtension : X509Extension
    {
        public X509BasicConstraintsExtension()
            : base(Oids.BasicConstraints2)
        {
            _decoded = true;
        }

        public X509BasicConstraintsExtension(bool certificateAuthority, bool hasPathLengthConstraint, int pathLengthConstraint, bool critical)
            : base(Oids.BasicConstraints2, EncodeExtension(certificateAuthority, hasPathLengthConstraint, pathLengthConstraint), critical)
        {
        }

        public X509BasicConstraintsExtension(AsnEncodedData encodedBasicConstraints, bool critical)
            : base(Oids.BasicConstraints2, encodedBasicConstraints.RawData, critical)
        {
        }

        public bool CertificateAuthority
        {
            get
            {
                if (!_decoded)
                    DecodeExtension();

                return _certificateAuthority;
            }
        }

        public bool HasPathLengthConstraint
        {
            get
            {
                if (!_decoded)
                    DecodeExtension();

                return _hasPathLenConstraint;
            }
        }

        public int PathLengthConstraint
        {
            get
            {
                if (!_decoded)
                    DecodeExtension();

                return _pathLenConstraint;
            }
        }

        public override void CopyFrom(AsnEncodedData asnEncodedData)
        {
            base.CopyFrom(asnEncodedData);
            _decoded = false;
            return;
        }

        private static byte[] EncodeExtension(bool certificateAuthority, bool hasPathLengthConstraint, int pathLengthConstraint)
        {
            if (hasPathLengthConstraint && pathLengthConstraint < 0)
                throw new ArgumentOutOfRangeException("pathLengthConstraint", SR.Arg_OutOfRange_NeedNonNegNum);

            return X509Pal.Instance.EncodeX509BasicConstraints2Extension(certificateAuthority, hasPathLengthConstraint, pathLengthConstraint);
        }

        private void DecodeExtension()
        {
            if (Oid.Value == Oids.BasicConstraints)
                X509Pal.Instance.DecodeX509BasicConstraintsExtension(this.RawData, out _certificateAuthority, out _hasPathLenConstraint, out _pathLenConstraint);
            else
                X509Pal.Instance.DecodeX509BasicConstraints2Extension(this.RawData, out _certificateAuthority, out _hasPathLenConstraint, out _pathLenConstraint);

            _decoded = true;
        }

        private bool _certificateAuthority = false;
        private bool _hasPathLenConstraint = false;
        private int _pathLenConstraint = 0;
        private bool _decoded = false;
    }
}
