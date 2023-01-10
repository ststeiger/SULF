#define FUSE_KERNEL_VERSION 6
#define FUSE_KERNEL_MINOR_VERSION 1
#define FUSE_ROOT_ID 1
#define FUSE_MAJOR 10
#define FUSE_MINOR 229
#define FATTR_MODE (1 << 0)
#define FATTR_UID (1 << 1)
#define FATTR_GID (1 << 2)
#define FATTR_SIZE (1 << 3)
#define FATTR_ATIME (1 << 4)
#define FATTR_MTIME (1 << 5)
#define FATTR_CTIME (1 << 6)
#define FUSE_MAX_IN 8192
#define FUSE_NAME_MAX 1024
#define FUSE_SYMLINK_MAX 4096
#define FUSE_XATTR_SIZE_MAX 4096
#define FUSE_NAME_OFFSET ((unsigned) ((struct fuse_dirent *) 0)->name)
#define FUSE_DIRENT_ALIGN(x) (((x) + sizeof(__u64) - 1) & ~(sizeof(__u64) - 1))
#define FUSE_DIRENT_SIZE(d) \
	FUSE_DIRENT_ALIGN(FUSE_NAME_OFFSET + (d)->namelen)
