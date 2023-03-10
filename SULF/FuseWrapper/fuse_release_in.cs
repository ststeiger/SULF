/* ----------------------------------------------------------------------------
 * This file was automatically generated by SWIG (http://www.swig.org).
 * Version 1.3.24
 *
 * Do not make changes to this file unless you know what you are doing--modify
 * the SWIG interface file instead.
 * ----------------------------------------------------------------------------- */


using System;

public class fuse_release_in : IDisposable, BufferReadable {
  private IntPtr swigCPtr;
  protected bool swigCMemOwn;

  internal fuse_release_in(IntPtr cPtr, bool cMemoryOwn) {
    swigCMemOwn = cMemoryOwn;
    swigCPtr = cPtr;
  }

  internal static IntPtr getCPtr(fuse_release_in obj) {
    return (obj == null) ? IntPtr.Zero : obj.swigCPtr;
  }

  ~fuse_release_in() {
    Dispose();
  }

  public virtual void Dispose() {
    if(swigCPtr != IntPtr.Zero && swigCMemOwn) {
      swigCMemOwn = false;
      FuseWrapperPINVOKE.delete_fuse_release_in(swigCPtr);
    }
    swigCPtr = IntPtr.Zero;
    GC.SuppressFinalize(this);
  }

  public ulong fh {
    get {
      return FuseWrapperPINVOKE.get_fuse_release_in_fh(swigCPtr);
    } 
  }

  public uint flags {
    get {
      return FuseWrapperPINVOKE.get_fuse_release_in_flags(swigCPtr);
    } 
  }

  public uint padding {
    get {
      return FuseWrapperPINVOKE.get_fuse_release_in_padding(swigCPtr);
    } 
  }

  public int copyFrom(FWBuffer buf) {
    return FuseWrapperPINVOKE.fuse_release_in_copyFrom(swigCPtr, FWBuffer.getCPtr(buf));
  }

  public fuse_release_in() : this(FuseWrapperPINVOKE.new_fuse_release_in(), true) {
  }

}
